using Couchbase.Lite.Sync;
using Codemancer.Extensions.Couchbase.Lite.Extensions;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public interface ISyncGateway : IDisposable
{
    public string  Name { get; }

    public Task SignInAsync(Credentials credentials, CancellationToken cancellationToken = default);

    public Task SignOutAsync(CancellationToken cancellationToken = default);

    public void Start();

    public void Stop();

    public void Resync(Action<Credentials, IReplicatorConfigurationBuilder> configure);
}

public class SyncGateway: ISyncGateway
{
    private Replicator? _replicator;
    private Credentials? _credentials;
    private readonly SyncOptions _options;
    private readonly ReplicatorConfiguration _config;
    private readonly ISessionService _sessionService;
    
    public SyncGateway(SyncOptions options, ISessionService sessionService)
    {
        _options = options;
        _sessionService = sessionService;
        _config = new ReplicatorConfiguration(new URLEndpoint(options.Endpoint));
    }

    public string Name { get { return _config.GetEndpointName(); } }

    public async Task SignInAsync(Credentials credentials, CancellationToken cancellationToken = default)
    {
        credentials ??= new AnonymousCredentials();
        if(credentials is JwtCredentials jwtCredentials)
        {
            var session = await _sessionService.CreateSessionAsync(_config.GetHttpEndpoint(), jwtCredentials.IdToken, cancellationToken);
            credentials = new SessionCredentials(session.Username, session.SessionId);
        }

        _config.Authenticator = Credentials.Create(credentials);

        var replicationBuiler = new ReplicatorConfigurationBuilder(_options.Database, _options.ScopeName, _config);
        _options.ConfigureReplication(credentials.Username, replicationBuiler);

        _credentials = credentials;

        //var resume = IsRunning(_replicator) || FailedToRun(_replicator);
        _replicator = Rebuild(_replicator, replicationBuiler.ReplicatorConfiguration, false);
        _replicator.Start();
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        if(_replicator is null)
        {
            return;
        }

        if (_replicator.Config.Authenticator is null)
        {
            return;
        }

        if (_replicator.Config.Authenticator is SessionAuthenticator)
        {
            await _sessionService.DeleteSessionAsync(_config.GetHttpEndpoint(), cancellationToken);
        }
        _config.Authenticator = null;

        Delete(_replicator);
        _replicator = null;
    }

    public void Start()
    {
        if(_replicator is null)
        {
            throw new Exception("Replicator is not initialized. Call SignedInAsync method to initialize the replicator. ");
        }
        _replicator.Start();
    }

    public void Stop()
    {
        if (_replicator is null)
        {
            return;
        }
        _replicator.Stop();
    }

    public void Resync(Action<Credentials, IReplicatorConfigurationBuilder> configure)
    {
        if(_replicator is null || _credentials is null)
        {
            throw new Exception("Replicator is not initialized. Call SignedInAsync method to initialize the replicator. ");
        }

        var replicationBuiler = new ReplicatorConfigurationBuilder(_options.Database, _options.ScopeName, _config);
        configure(_credentials, replicationBuiler);

        _replicator = Rebuild(_replicator, replicationBuiler.ReplicatorConfiguration, true);
    }

    public void Dispose()
    {
        if(_replicator is null)
        {
            return;
        }
        Delete(_replicator);
        _replicator = null;
    }

    private Replicator Rebuild(Replicator? replicator, ReplicatorConfiguration config, bool autoStart)
    {
        if (replicator is not null)
        {
            Delete(replicator);
            replicator = null;
        }

        return Create(config, autoStart);
    }

    private Replicator Create(ReplicatorConfiguration config, bool autoStart)
    {
        var replicator = new Replicator(config);
        replicator.AddChangeListener(_options.Events.OnStatusChanged);
        if (autoStart)
        {
            replicator.Start();
        }
        return replicator;
    }

    private void Delete(Replicator replicator)
    {
        replicator.Stop();
        replicator.Dispose();
    }

    private bool IsRunning(Replicator? replicator)
    {
        if(replicator is null)
        {
            return false;
        }

        return replicator.Status.Activity != ReplicatorActivityLevel.Stopped;
    }

    private bool FailedToRun(Replicator? replicator)
    {
        if (replicator is null)
        {
            return false;
        }
        return replicator.Status.Activity == ReplicatorActivityLevel.Stopped && replicator.Status.Error != null;
    }

}

