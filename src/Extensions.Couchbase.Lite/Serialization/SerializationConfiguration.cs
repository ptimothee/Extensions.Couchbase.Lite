using System.Text.Json;
using System.Text.Json.Serialization;
using Codemancer.Extensions.Couchbase.Lite.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite;

public static class SerializationConfiguration
{
    public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new BlobConverter() }
    };
}
