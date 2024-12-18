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

            // When no value
            if (value is null) 
            {
                continue; // Ignore
            }

            // When value is a complex type
            if (value is DictionaryObject dictionary)
            {
                // Deconstruct each field and but promote field names to root object
                foreach (var item in dictionary) 
                {
                    if (item.Value is DictionaryObject dobj)
                    {
                        var json = JsonSerializer.Serialize(dobj.ToDictionary(), SerializationConfiguration.DefaultOptions);
                        jsonResult[item.Key] = JsonNode.Parse(json);
                        continue;
                    }

                    if (item.Value is ArrayObject arr)
                    {
                        var json = JsonSerializer.Serialize(arr.ToList(), SerializationConfiguration.DefaultOptions);
                        jsonResult[item.Key] = JsonNode.Parse(json);
                        continue;
                    }

                    if (value is Blob blb)
                    {
                        jsonResult[key] = JsonValue.Create(Convert.ToBase64String(blb.Content ?? Array.Empty<byte>()));
                        continue;
                    }

                    jsonResult[item.Key] = JsonValue.Create(item.Value);
                }
                continue;
            }

            // When value is a sequence
            if (value is ArrayObject array)
            {             
                var json = JsonSerializer.Serialize(array.ToList(), SerializationConfiguration.DefaultOptions);
                jsonResult[key] = JsonArray.Parse(json);
                continue;
            }

            // ... value is a simple type
            if (value is Blob blob)
            {
                jsonResult[key] = JsonValue.Create(Convert.ToBase64String(blob.Content ?? Array.Empty<byte>()));
                continue;
            }

            jsonResult[key] = JsonValue.Create(value);
            continue;
        }

        if(!jsonResult.Any())
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(jsonResult, SerializationConfiguration.DefaultOptions);    
    }
}
