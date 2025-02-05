using Couchbase.Lite;
using Couchbase.Lite.Sync;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using Codemancer.Extensions.Couchbase.Lite.Extensions;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public interface ISyncGateway : IAsyncDisposable
{
    public string  Name { get; }

    public IPrincipal? User { get; }

    public Task SignInAsync(Credentials credentials, CancellationToken cancellationToken = default);

    public Task SignOutAsync(CancellationToken cancellationToken = default);

    public Task StartAsync();

    public Task StopAsync();

    public Task Resync(Action<IReplicatorConfigurationBuilder, SyncSessionContext> configure);
}

public class SyncGateway: ISyncGateway
{
    private Replicator? _replicator;
    private readonly SyncOptions _options;
    private ReplicatorConfiguration _config;
    private readonly Database _database;
    private readonly IServiceProvider _serviceProvider;

    public SyncGateway(ReplicatorConfiguration replicatorConfig, SyncOptions options, Database database, IServiceProvider serviceProvider)
    {
        _options = options;
        _config = replicatorConfig;
        _database = database;
        _serviceProvider = serviceProvider;
    }

    public string Name { get { return _config.GetEndpointName(); } }

    public IPrincipal? User { get; private set; }

    public async Task SignInAsync(Credentials credentials, CancellationToken cancellationToken = default)
    {
        credentials ??= new AnonymousCredentials();
        if(credentials is JwtCredentials jwtCredentials)
        {
            if (jwtCredentials.AuthenticationMethod == AuthenticationMethod.SessionProvider)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                    var session = await sessionService.CreateSessionAsync(_config.GetHttpEndpoint(), jwtCredentials.IdToken, cancellationToken)
                                                        .ConfigureAwait(false);

                    credentials = new SessionCredentials(session.Username, session.SessionId);
                }
            }
            else
            {
                _config.Headers.Add("Authorization", $"Bearer {jwtCredentials.IdToken}");
            }
        }

        _config.Authenticator = Credentials.Create(credentials);
        User = new GenericPrincipal(new GenericIdentity(credentials.Username), Array.Empty<string>());

        var replicationBuiler = new ReplicatorConfigurationBuilder(_database, _options.ScopeName, _config);
        using (var scope = _serviceProvider.CreateScope())
        {
            _options.ConfigureReplication(replicationBuiler, new SyncSessionContext(User, scope.ServiceProvider));
        }

        _replicator = Create(replicationBuiler.ReplicatorConfiguration);
        await _replicator.StartAsync().ConfigureAwait(false);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        if(_replicator is null)
        {
            return;
        }

        try
        {
            await _replicator.StopAsync().ConfigureAwait(false);

            if (_replicator.Config.Authenticator is null)
            {
                if (_replicator.Config.Headers.ContainsKey("Authorization"))
                {
                    _replicator.Config.Headers.Remove("Authorization");
                }
                return;
            }

            if (_replicator.Config.Authenticator is SessionAuthenticator)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                    await sessionService.DeleteSessionAsync(_replicator.Config.GetHttpEndpoint(), cancellationToken)
                                        .ConfigureAwait(false);
                }
            }
            _config.Authenticator = null;
        }
        finally
        {
            User = null;
            _replicator.Dispose();
            _replicator = null;
        }
    }

    public Task StartAsync()
    {
        if (_replicator is null || User is null)
        {
            throw new Exception("Replicator is not initialized. Call SignedInAsync method to authenticate and initialize the replicator. ");
        }
        return _replicator.StartAsync();
    }

    public Task StopAsync()
    {
        if (_replicator is null)
        {
            return Task.CompletedTask;
        }
        return _replicator.StopAsync();
    }

    public async Task Resync(Action<IReplicatorConfigurationBuilder, SyncSessionContext> configure)
    {
        if (_replicator is null || User is null)
        {
            throw new Exception("Replicator is not initialized. Call SignedInAsync method to authenticate and initialize the replicator. ");
        }

        var replicationBuiler = new ReplicatorConfigurationBuilder(_database, _options.ScopeName, _config);
        using (var scope = _serviceProvider.CreateScope())
        {   
            configure(replicationBuiler, new SyncSessionContext(User, scope.ServiceProvider));
        }

        await _replicator.StopAsync().ConfigureAwait(false);
        _replicator.Dispose();
        _replicator = null;
        _replicator = Create(replicationBuiler.ReplicatorConfiguration);
        await _replicator.StartAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_replicator is null)
        {
            return;
        }
        await _replicator.StopAsync().ConfigureAwait(false);
        _replicator.Dispose();
        _replicator = null;
    }

    private Replicator Create(ReplicatorConfiguration config)
    {
        var replicator = new Replicator(config);
        replicator.AddChangeListener(_options.Events.OnStatusChanged);
        return replicator;
    }
}
