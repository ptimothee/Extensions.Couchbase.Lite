using System.Security.Principal;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public record SyncSessionContext(IPrincipal Principal, IServiceProvider Services);
