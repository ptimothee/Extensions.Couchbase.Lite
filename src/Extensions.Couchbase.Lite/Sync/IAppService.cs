namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public interface IAppService
{
    public IEnumerable<ISyncGateway> Gateways { get; }

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

    public IEnumerable<ISyncGateway> Gateways { get { return _namedGateways.Values; } }
}

