using Couchbase.Lite.Sync;
using System.IdentityModel.Tokens.Jwt;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public abstract class Credentials
{
    public string Username { get; protected set; } = string.Empty;

    public static Credentials CreateAnonymous() => new AnonymousCredentials();

    public static Credentials CreateBasic(string username, string password) => new BasicCredentials(username, password);

    public static Credentials CreateJwt(string idToken, AuthenticationMethod authenticationMethod = AuthenticationMethod.SessionProvider, Action<UsernameBuildOptions>? configure = null)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(idToken);

        var options = new UsernameBuildOptions("sub", token.Issuer);
        configure?.Invoke(options);

        return new JwtCredentials(token, authenticationMethod, options);
    }

    internal static Authenticator? Create(Credentials credentials)
    {
        if (credentials is AnonymousCredentials)
        {
            return null;
        }

        if (credentials is BasicCredentials basicCredentials)
        {
            return new BasicAuthenticator(basicCredentials.Username, basicCredentials.Password);
        }

        if (credentials is SessionCredentials sessionCredentials)
        {
            return new SessionAuthenticator(sessionCredentials.SessionId);
        }

        if(credentials is JwtCredentials jwtCredentials && jwtCredentials.AuthenticationMethod == AuthenticationMethod.AuthorizationHeader)
        {
            return null;
        }

        throw new NotSupportedException($"Unsupported credentials type: {typeof(Credentials).FullName}. ");
    }
}

public sealed class AnonymousCredentials : Credentials
{
    internal AnonymousCredentials()
    {
        Username = "anonymous";
    }
}

public sealed class BasicCredentials : Credentials
{
    internal BasicCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Password { get; }
}

public sealed class JwtCredentials : Credentials
{
    internal JwtCredentials(JwtSecurityToken token, AuthenticationMethod authenticationMethod, UsernameBuildOptions options)
    {
        if (!token.Payload.TryGetValue(options.ClaimType, out var claimValue))
        {
            throw new InvalidOperationException($"ClaimType: '{options .ClaimType}' not found in supplied JWT token. ");
        }

        Username = $"{options.Prefix}_{claimValue}";
        IdToken = token.RawData;
        Options = options;
    }

    public string IdToken { get; }

    public AuthenticationMethod AuthenticationMethod { get; }

    private UsernameBuildOptions Options { get; }
}

public sealed class SessionCredentials : Credentials
{
    internal SessionCredentials(string username, string sessionId)
    {
        Username = username;
        SessionId = sessionId;
    }

    public string SessionId { get; }
}

