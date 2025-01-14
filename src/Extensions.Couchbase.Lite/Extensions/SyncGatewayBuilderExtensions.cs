using Couchbase.Lite;
using Couchbase.Lite.Sync;
using Codemancer.Extensions.Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Extensions;

public static class SyncGatewayBuilderExtensions
{
    public static IReplicatorConfigurationBuilder LinkCollection(this IReplicatorConfigurationBuilder builder, string collectionName, string[] channels)
    {
        LinkCollection(builder, collectionName, collectionConfig => collectionConfig.Channels = channels);

        return builder;
    }

    public static IReplicatorConfigurationBuilder LinkCollection(this IReplicatorConfigurationBuilder builder, string collectionName, Action<CollectionConfiguration> configure )
    {
        Database db = builder.Database;

        var collection = db.GetCollection(collectionName, builder.ScopeName);
        if (collection is null)
        {
            collection = db.CreateCollection(collectionName, builder.ScopeName);
        }

        var collectionConfiguration = builder.ReplicatorConfiguration.GetCollectionConfig(collection);
        if(collectionConfiguration is null)
        {
            collectionConfiguration = new CollectionConfiguration();
        }

        configure(collectionConfiguration);

        if(collectionConfiguration.Channels is not null && !collectionConfiguration.Channels.Any())
        {
            collectionConfiguration.Channels = null;
        }

        builder.ReplicatorConfiguration.AddCollection(collection, collectionConfiguration);

        return builder;
    }

    public static IReplicatorConfigurationBuilder UnlinkCollection(this IReplicatorConfigurationBuilder builder, string collectionName)
    {
        Database db = builder.Database;

        var collection = db.GetCollection(collectionName, builder.ScopeName);
        if (collection is null)
        {
            return builder;
        }

        builder.ReplicatorConfiguration.RemoveCollection(collection);

        return builder;
    }
}
