using Couchbase.Lite;
using Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public class SyncOptions
{
    public SyncOptions(Uri uri, Database database)
    {
        Endpoint = uri;
        Database = database;
    }

    public Uri Endpoint { get; }

    public Database Database { get; set; }

    public Action<string, IReplicatorConfigurationBuilder> ConfigureReplication { get; set; } = (scopeName, builder) => { };

    public SyncEvents Events { get; set; } = new SyncEvents();
}

public class SyncEvents
{
    public EventHandler<ReplicatorStatusChangedEventArgs> OnStatusChanged { get; set; } = (sender, args) => { };

    public EventHandler<DocumentReplicationEventArgs> OnDocumentReplicated { get; set; } = (sender, args) => { };
}

