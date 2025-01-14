using Couchbase.Lite;
using Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public interface IReplicatorConfigurationBuilder
{
    public ReplicatorConfiguration ReplicatorConfiguration { get; }

    public string ScopeName { get; }

    public Database Database { get; }
}

internal class ReplicatorConfigurationBuilder : IReplicatorConfigurationBuilder
{
    public ReplicatorConfigurationBuilder(Database database, string scopeName, ReplicatorConfiguration replicatorConfiguration)
    {
        Database = database;
        ScopeName = scopeName;
        ReplicatorConfiguration = replicatorConfiguration;
    }

    public ReplicatorConfiguration ReplicatorConfiguration { get; }

    public Database Database { get; }

    public string ScopeName { get; }
}
