using Couchbase.Lite;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite.Serialization;

public class BlobConverter : JsonConverter<Blob>
{
    public override Blob Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.Read())
        {
            return new Blob(string.Empty, reader.GetBytesFromBase64());
        }

       return new Blob(string.Empty, []);   
    }

    public override void Write(Utf8JsonWriter writer, Blob value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Convert.ToBase64String(value.Content ?? []));
    }
}
