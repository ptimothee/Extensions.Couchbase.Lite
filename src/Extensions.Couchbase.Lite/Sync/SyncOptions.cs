using Couchbase.Lite;
using Couchbase.Lite.Sync;
using System.Security.Principal;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public class SyncOptions
{
    public string ScopeName { get; set; } = Collection.DefaultScopeName;

    public Action<IList<DelegatingHandler>, IServiceProvider> ConfigureSessionDelegatingHandler { get; set; } = (handlers, sp) => { };

    public Action<IPrincipal, IReplicatorConfigurationBuilder, IServiceProvider> ConfigureReplication { get; set; } = (principal, builder, sp) => { };

    public SyncEvents Events { get; set; } = new SyncEvents();
}

public class SyncEvents
{
    public EventHandler<ReplicatorStatusChangedEventArgs> OnStatusChanged { get; set; } = (sender, args) => { };

    public EventHandler<DocumentReplicationEventArgs> OnDocumentReplicated { get; set; } = (sender, args) => { };
}

