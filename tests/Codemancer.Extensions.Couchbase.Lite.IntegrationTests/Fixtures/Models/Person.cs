using System.Text.Json.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures.Models;

public class Person
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }
}
