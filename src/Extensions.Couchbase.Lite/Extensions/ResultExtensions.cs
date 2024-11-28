using Couchbase.Lite;
using Couchbase.Lite.Query;
using System.Text.Json;

namespace Codemancer.Extensions.Couchbase.Lite;

public static class ResultExtensions
{
    public static T ToObject<T>(this Result result)
    {
        if (result is null)
        {
            return default!;
        }

        foreach (var key in result.Keys)
        {
            var value = result.GetValue(key);
            var value1 = result[key];

            if (value is DictionaryObject obj)
            {
                var data = obj.ToDictionary();

                var json = JsonSerializer.Serialize(data);
                var x = JsonSerializer.Deserialize<T>(json);
            }

            Console.WriteLine(key);
        }

        throw new NotImplementedException();
    }
}
