using Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Extensions
{
    internal static class Extensions
    {
        public static Uri AsHttpUri(this IEndpoint endpoint)
        {
            if(endpoint is not URLEndpoint urlEndpoint)
            {
                throw new InvalidOperationException("Endpoint is not a URLEndpoint.");
            }

            // Ensure trailing slash
            var path = urlEndpoint.Url.LocalPath;
            if (!path.EndsWith('/'))
            {
                path += '/';
            }

            var uriBuilder = new UriBuilder(urlEndpoint.Url)
            {
                Scheme = "https", // Switch scheme to https
                Path = path
            };

            return uriBuilder.Uri;
        }
    }
}
