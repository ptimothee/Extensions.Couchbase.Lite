using Couchbase.Lite.Query;

namespace Codemancer.Extensions.Couchbase.Lite;

public static class QueryExtensions
{
    public static IEnumerable<T> Execute<T>(this IQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var results = new CouchbaseLiteLiveResults<T>(query);

        return results;
    }
}
