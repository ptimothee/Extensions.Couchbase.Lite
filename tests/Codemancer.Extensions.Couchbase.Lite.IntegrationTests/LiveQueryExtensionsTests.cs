using FluentAssertions;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures.Models;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Extensions;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests;

[Collection("Database collection")]
public class LiveQueryExtensionsTests: IAsyncLifetime
{
    public LiveQueryExtensionsTests(DatabaseFixture databaseFixture)
    {
        Collection = databaseFixture.DB.CreateCollection("People", nameof(LiveQueryExtensionsTests));
    }

    public Collection Collection { get; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    [Trait("category", "integration-test")]
    public async Task LiveQueryResult_WhenDataModified_ShouldReflectChanges()
    {
        // Arrange
        IQuery query = Collection.CreateQuery($"SELECT * FROM {Collection.FullName} WHERE age = 40");

        var results = query.Execute<Person>();

        // Act
        var liveCollection = results.AsLiveCollection();

        Collection.Save(new Person { Name = "Julie", Age = 40 }.ToMutableDocument());

        await Task.Delay(TimeSpan.FromSeconds(1)); // Hack to wait for the query to update

        // Assert
        liveCollection.Should().HaveCount(1);

        liveCollection.Dispose();
    }

    public Task DisposeAsync()
    {
        Collection.Drop();

        return Task.CompletedTask;
    }
}
