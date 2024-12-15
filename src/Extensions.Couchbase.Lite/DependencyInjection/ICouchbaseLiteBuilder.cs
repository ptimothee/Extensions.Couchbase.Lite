using Microsoft.Extensions.DependencyInjection;

namespace Codemancer.Extensions.Couchbase.Lite.DependencyInjection;

public interface ICouchbaseLiteBuilder
{
    public IServiceCollection Services { get; }
}

internal class CouchbaseLiteBuilder : ICouchbaseLiteBuilder
{
    public CouchbaseLiteBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
