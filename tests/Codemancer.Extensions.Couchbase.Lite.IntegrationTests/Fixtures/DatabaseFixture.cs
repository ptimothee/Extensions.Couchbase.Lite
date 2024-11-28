using Couchbase.Lite;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        Database = new Database("test-database");
        Database.Delete();
        Database = new Database("test-database");
    }

    public Database Database { get; }

    public void Dispose()
    {
        Database.Delete();
        Database.Dispose();
    }
}
