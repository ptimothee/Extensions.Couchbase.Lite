using AutoFixture;
using FluentAssertions;
using Couchbase.Lite.Query;
using Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Models;
using Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Customizations;

namespace Codemancer.Extensions.Couchbase.Lite.Tests;

public class ExtensionsTests
{
    [Fact]
    [Trait("category", "unit-test")]
    public void ExecuteAsync_WhenQueryExecutionFailed_ShouldThrowException()
    {
        //Arrange
        var fixture = new Fixture();
        fixture.Customize(
            new FailedQueryCustomization(
                new Exception("Whoops!")
            )
        );

        var query = fixture.Create<IQuery>();

        // Act
        Func<IEnumerable<Person>> func = () => query.Execute<Person>();

        // Assert
        func.Should()
            .Throw<Exception>()
            .WithMessage("Whoops!");
    }

    [Fact]
    [Trait("category", "unit-test")]
    public void LiveCollection_WhenQueryChangedEventHasError_OperatingOnLiveCollectionShouldThrowException()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize(
            new LiveQueryDispatchingFailedQueryResultChangedCustomization(
                new Exception("Whoops!")
            )
        );

        var query = fixture.Create<IQuery>();

        // Act
        using var liveResults = query.Execute<Person>().AsLiveCollection();

        // Assert
        Action action = () => liveResults.Count();

        action.Should()
                .Throw<Exception>()
                .WithMessage("Whoops!");
    }
}