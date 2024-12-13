using FluentAssertions;
using Couchbase.Lite;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures.Models;
using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Extensions;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests;

[Collection("Database collection")]
public class QueryExtensionsTests : IAsyncLifetime
{
    public QueryExtensionsTests(DatabaseFixture databaseFixture)
    {
        Collection = databaseFixture.DB.CreateCollection("People", nameof(QueryExtensionsTests));
    }

    public Collection Collection { get; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory]
    [Trait("category", "integration-test")]
    [MemberData(nameof(TestDataFixture.GetPeople), MemberType = typeof(TestDataFixture))]
    public void ExecuteOnTypeModel_ShouldSerializeResult(Person expectedPerson)
    {
        // Arrange
        Collection.Save(expectedPerson.ToMutableDocument());

        var sql = $"SELECT meta().id, * FROM  {Collection.FullName} WHERE name ='{expectedPerson.Name}'";
        var query = Collection.CreateQuery(sql);

        // Act
        var results = query.Execute<Person>();

        // Assert
        results.Should().HaveCount(1);

        var actualPerson = results.Single();
        actualPerson.Id.Should().NotBeNullOrEmpty();
        actualPerson.Should().BeEquivalentTo(expectedPerson, options => options.Excluding(p => p.Id));
    }

    public Task DisposeAsync()
    {
        Collection.Drop();

        return Task.CompletedTask;
    }
}
