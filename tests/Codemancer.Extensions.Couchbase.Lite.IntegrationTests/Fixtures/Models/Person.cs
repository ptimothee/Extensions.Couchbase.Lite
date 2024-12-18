using System.Text.Json.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures.Models;

public class Person
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("pet")]
    public Pet? Pet { get; set; }

    [JsonPropertyName("dob")]
    public DateTime? Dob { get; set; }

    [JsonPropertyName("photo")]
    public Byte[]? Photo { get; set; }

    [JsonPropertyName("digitalProfiles")]
    public DigitalProfile[] DigitalProfiles { get; set; } = [];
}

public class DigitalProfile
{
    [JsonPropertyName("siteName")]
    public string SiteName { get; set; } = string.Empty;
    [JsonPropertyName("webAddress")]
    public string WebAddress { get; set; } = string.Empty;
}

public class Pet
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("photo")]
    public Byte[]? Photo { get; set; }
}
