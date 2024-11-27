using Couchbase.Lite;
using Couchbase.Lite.Sync;
using Codemancer.Extensions.Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Extensions;

public static class SyncGatewayBuilderExtensions
{
    public static IReplicatorConfigurationBuilder UseScope(this IReplicatorConfigurationBuilder builder, string scopeName)
    {
        return new ReplicatorConfigurationBuilder(builder.Database, builder.ReplicatorConfiguration, scopeName);
    }

    public static IReplicatorConfigurationBuilder LinkCollection(this IReplicatorConfigurationBuilder builder, string collectionName, string[] channels)
    {
        Database db = builder.Database;

        var collection = db.GetCollection(collectionName, builder.ScopeName);
        if (collection is null)
        {
            collection = db.CreateCollection(collectionName, builder.ScopeName);
        }

        List<string>? channelsList = channels.Any() ? channels.ToList() : null;
        var collectionConfiguration = new CollectionConfiguration
        {
            Channels = channelsList
        };        

        builder.ReplicatorConfiguration.AddCollection(collection, collectionConfiguration);

        return builder;
    }
}
