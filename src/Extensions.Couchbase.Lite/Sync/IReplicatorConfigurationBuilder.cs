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
    public ReplicatorConfigurationBuilder(Database database, ReplicatorConfiguration replicatorConfiguration, string? scopeName = null)
    {
        Database = database;
        ReplicatorConfiguration = replicatorConfiguration;
        if (scopeName is null)
        {
            ScopeName = database.GetDefaultScope().Name;
        }
        else
        {
            ScopeName = scopeName;
        }
    }

    public ReplicatorConfiguration ReplicatorConfiguration { get; }

    public Database Database { get; }

    public string ScopeName { get; }
}
