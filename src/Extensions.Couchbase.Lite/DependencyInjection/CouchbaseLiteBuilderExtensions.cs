using Couchbase.Lite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Codemancer.Extensions.Couchbase.Lite.Sync;
using Couchbase.Lite.Sync;
using Codemancer.Extensions.Couchbase.Lite.Extensions;

namespace Codemancer.Extensions.Couchbase.Lite.DependencyInjection;

public static class CouchbaseLiteBuilderExtensions
{
    public static ICouchbaseLiteBuilder AddCouchbaseLite(this IServiceCollection services, string databaseName, Action<DatabaseBuildOptions>? configure = null)
    {
        DatabaseBuildOptions? options = null;
        if (configure is not null)
        {
            options = new();
            configure(options);
        }
        
        services.AddSingleton<Database>(sp =>
        {
            var database = new Database(databaseName, options?.Config);

            options?.OnBuildDatabase(database);
           
            return database;
        });

        return new CouchbaseLiteBuilder(services);
    }

    public static ICouchbaseLiteBuilder WithSyncGateway(this ICouchbaseLiteBuilder builder, Uri uri, Action<SyncOptions> configure)
    {
        var services = builder.Services;

        var options = new SyncOptions();
        configure(options);

        var replicatorConfiguration = new ReplicatorConfiguration(new URLEndpoint(uri));

        var httpClientBuilder = services.AddHttpClient(replicatorConfiguration.GetEndpointName())
                                        .ConfigureAdditionalHttpMessageHandlers(options.ConfigureSessionDelegatingHandler);
        
        services.TryAddSingleton<ISessionService, SessionService>();

        services.AddSingleton<ISyncGateway>(sp =>
        {
            var database = sp.GetRequiredService<Database>();
            var sessionService = sp.GetRequiredService<ISessionService>();
            
            return new SyncGateway(replicatorConfiguration, options, database, sessionService);
        });

        services.TryAddSingleton<IAppService>(sp =>
        {
            var gateways = sp.GetRequiredService<IEnumerable<ISyncGateway>>();
            var map = gateways.ToDictionary(gw => gw.Name, gw => gw);

            return new AppService(map);
        });

        return builder;
    }

}


