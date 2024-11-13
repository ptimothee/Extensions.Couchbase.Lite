namespace Codemancer.Extensions.Couchbase.Lite;

public static class EnumerableExtensions
{
    public static CouchbaseLiteLiveResults<T> AsLiveCollection<T>(this IEnumerable<T> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var couchbaseLiteResults = results.EnsureType<CouchbaseLiteLiveResults<T>>($"{nameof(results)} must be of type IEnumerable<{typeof(T).Name}>. ", nameof(results));

        couchbaseLiteResults.SubscribeToLiveQuery();

        return couchbaseLiteResults;
    }

    private static T EnsureType<T>(this object obj, string message, string paramName)
    {
        if (obj is not T tObj)
        {
            throw new ArgumentException(message, paramName);
        }

        return tObj;
    }
}
