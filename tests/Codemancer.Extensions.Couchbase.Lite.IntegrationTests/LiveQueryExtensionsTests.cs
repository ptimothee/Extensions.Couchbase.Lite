using FluentAssertions;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures.Models;

namespace Codemancer.Extensions.Couchbase.Lite.Tests;

public class LiveQueryExtensionsTests: IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    public LiveQueryExtensionsTests(DatabaseFixture databaseFixture)
    {
        Collection = databaseFixture.Database.CreateCollection("People", nameof(LiveQueryExtensionsTests));
    }

    public Collection Collection { get; }

    public Task InitializeAsync()
    {
        Collection.Save(new Person { Name = "John", Age = 45 }.ToMutableDocument());
        Collection.Save(new Person { Name = "Jane", Age = 40 }.ToMutableDocument());
        Collection.Save(new Person { Name = "Jerry", Age = 40 }.ToMutableDocument());
        Collection.Save(new Person { Name = "Jill", Age = 35 }.ToMutableDocument());
        Collection.Save(new Person { Name = "Jack", Age = 30 }.ToMutableDocument());

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
        liveCollection.Should().HaveCount(3);
    }

    public Task DisposeAsync()
    {
        Collection.Dispose();
        return Task.CompletedTask;
    }
}
