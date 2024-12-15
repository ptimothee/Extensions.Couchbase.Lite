using Couchbase.Lite;
using Couchbase.Lite.Query;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Codemancer.Extensions.Couchbase.Lite;

public static class ResultExtensions
{
    public static T? ToObject<T>(this Result result)
    {
        if (result is null)
        {
            return default;
        }

        var jsonResult = new JsonObject();
        foreach (var key in result.Keys)
        {
            var value = result[key]?.Value;
            if (value is null) // When no value
            {
                continue; // Ignore
            }

            if (value is DictionaryObject dictionary) // When Complex type
            {             
                foreach (var item in dictionary) // Add all key:value fields
                {
                    if(item.Value.TryConvertAsDictionary(out var dict))
                    {
                        var json = JsonSerializer.Serialize(dict, SerializationConfiguration.DefaultOptions);
                        jsonResult[item.Key] = JsonNode.Parse(json);
                    }
                    else
                    {
                        jsonResult[item.Key] = JsonValue.Create(item.Value);
                    }
                }
                continue;
            }

            if (value is Blob blob)
            {
                jsonResult[key] = JsonValue.Create(Convert.ToBase64String(blob.Content ?? Array.Empty<byte>()));
                continue;
            }

            //When simple value type
            jsonResult[key] = JsonValue.Create(value);
            continue;
        }

        if(!jsonResult.Any())
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(jsonResult, SerializationConfiguration.DefaultOptions);    
    }

    private static bool TryConvertAsDictionary(this object? obj, out Dictionary<string, object?>? dictionary)
    {
        if (obj is DictionaryObject dObj) // Complex Type
        {
            dictionary = dObj.ToDictionary();
            return true;
        }

        dictionary = null;
        return false;
    }
}
