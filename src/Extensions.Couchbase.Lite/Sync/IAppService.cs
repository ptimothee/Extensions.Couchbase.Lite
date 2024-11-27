namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public interface IAppService
{
    public ISyncGateway GetSyncGateway(string endpointName);
}

public class AppService(Dictionary<string, ISyncGateway> namedGateways) : IAppService
{
    private readonly Dictionary<string, ISyncGateway> _namedGateways = namedGateways;

    public ISyncGateway GetSyncGateway(string endpointName)
    {
        if(_namedGateways.TryGetValue(endpointName, out var gateway))
        {
            return gateway;
        }

        throw new InvalidOperationException($"No gateway registered for endpoint name '{endpointName}'. ");
    }
}

