using Couchbase.Lite;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        DB = new Database("test-database");
    }

    public Database DB { get; }

    public void Dispose()
    {
        DB.Dispose();
        Database.Delete("test-database", null);
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
