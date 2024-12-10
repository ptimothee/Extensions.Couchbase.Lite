using FluentAssertions;
using Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures;
using Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Models;

namespace Codemancer.Extensions.Couchbase.Lite.Tests;

public class ObjectExtensionsTests
{
    [Fact]
    [Trait("category", "unit-test")]
    public void ToMutableDocument_WhenInstanceIsNull_ShouldReturnMutableDocumentWithGeneratedId()
    {
        // Arrange
        Person? person = null;

        // Act
        var document = person.ToMutableDocument();

        // Assert
        document.Id.Should().NotBeNullOrEmpty();
        document.Keys.Should().BeEmpty();
    }

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