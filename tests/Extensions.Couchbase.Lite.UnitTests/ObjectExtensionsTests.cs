using FluentAssertions;
using Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures;

namespace Codemancer.Extensions.Couchbase.Lite.Tests;

public class ObjectExtensionsTests
{
    [Theory]
    [Trait("category", "unit-test")]
    [MemberData(nameof(ObjectExtensionsTestFixture.GetSupportedMutableObjects), MemberType = typeof(ObjectExtensionsTestFixture))]
    public void ToMutableDocument_ShouldReturnMutableDocumentWithExpected(object? obj, Dictionary<string, object?> expectedDictionary)
    {
        // Act
        var document = obj.ToMutableDocument();

        // Assert
        document.Id.Should().NotBeNullOrEmpty();
        document.ToDictionary().Should().BeEquivalentTo(expectedDictionary);
    }
}