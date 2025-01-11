using Couchbase.Lite;

namespace Codemancer.Extensions.Couchbase.Lite
{
    public class DatabaseBuildOptions
    {
        public DatabaseConfiguration Config { get; set; } = new DatabaseConfiguration();

        public Action<Database> OnBuildDatabase { get; set; } = db => { };
    }
}
