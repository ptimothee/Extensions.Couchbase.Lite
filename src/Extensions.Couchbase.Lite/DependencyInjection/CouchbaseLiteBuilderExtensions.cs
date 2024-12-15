using Couchbase.Lite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Codemancer.Extensions.Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.DependencyInjection;

public static class CouchbaseLiteBuilderExtensions
{
    public static ICouchbaseLiteBuilder AddCouchbaseLite(this IServiceCollection services, string databaseName, Action<DatabaseConfiguration>? configure = null)
    {
        DatabaseConfiguration? configuration = null;
        if (configure is not null)
        {
            configuration = new();
            configure(configuration);
        }
        
        services.AddSingleton<Database>(sp =>
        {
            return new Database(databaseName, configuration);
        });

        return new CouchbaseLiteBuilder(services);
    }

    public static ICouchbaseLiteBuilder WithSyncGateway(this ICouchbaseLiteBuilder builder, Uri uri, Action<SyncOptions> configure)
    {
        var services = builder.Services;

        string endpointName = uri.Segments.Last().Trim('/');

        //TODO: provide hook configure message handler
        services.AddHttpClient();

        services.TryAddSingleton<ISessionService, SessionService>();

        services.AddKeyedSingleton<ISyncGateway>(endpointName, (sp, key) =>
        {
            var name = Convert.ToString(key)!;

            var database = sp.GetRequiredService<Database>();
            var sessionService = sp.GetRequiredService<ISessionService>();
            var options = new SyncOptions(uri, database);

            configure(options);

            return new SyncGateway(name, options, sessionService);
        });

        services.TryAddSingleton<IAppService>(sp =>
        {
            var keys = services.Where(sd => sd.IsKeyedService && sd.ServiceType == typeof(ISyncGateway))
                                .Select(x => x.ServiceKey!.ToString()!);

            Dictionary<string, ISyncGateway> map = new(keys.Count());
            foreach (var key in keys)
            {
                var service = sp.GetRequiredKeyedService<ISyncGateway>(key);
                map.Add(key, service);
            }
            return new AppService(map);
        });

        return builder;
    }

}


