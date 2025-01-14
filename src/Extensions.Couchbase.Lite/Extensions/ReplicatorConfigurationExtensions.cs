using Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Extensions
{
    public static class ReplicatorConfigurationExtensions
    {
        public static Uri GetHttpEndpoint(this ReplicatorConfiguration configuration)
        {
            return configuration.Target.AsHttpUri();
        }

        public static string GetEndpointName(this ReplicatorConfiguration configuration)
        {
            if (configuration.Target is not URLEndpoint urlEndpoint)
            {
                throw new InvalidOperationException("Replicator target is not a URLEndpoint");
            }
            return urlEndpoint.Url.GetEndpointName();
        }

        private static Uri AsHttpUri(this IEndpoint endpoint)
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

        internal static string GetEndpointName(this Uri uri)
        {
            return uri.Segments.Last().Trim('/');
        }
    }
}
