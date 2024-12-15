using Couchbase.Lite;
using Couchbase.Lite.Query;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Extensions
{
    public static class CollectionExtensions
    {
        public static void Drop(this Collection collection)
        {
            var sql = $"SELECT meta().id FROM {collection.FullName}";

            IQuery query = collection.Database.CreateQuery(sql);

            var resultSet = query.Execute();

            foreach (var result in resultSet)
            {
                var documentId = result.GetString("id");
                var document = collection.GetDocument(documentId!);
                collection.Delete(document!);
            }

            collection.Database.DeleteCollection(collection.Name);

        }
    }
}
