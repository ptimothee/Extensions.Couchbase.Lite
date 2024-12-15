using System.Runtime.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Models;

public enum Sizes
{
    [EnumMember(Value = "small")]
    Small,
    [EnumMember(Value = "medium")]
    Medium,
    [EnumMember(Value = "large")]
    Large
}
