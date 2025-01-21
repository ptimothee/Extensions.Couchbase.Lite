namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public class UsernameBuildOptions(string claimName, string prefix)
{
    public string Prefix { get; set; } = prefix;

    public string ClaimType { get; set; } = claimName;
}

