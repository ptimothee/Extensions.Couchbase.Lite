using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace Codemancer.Extensions.Couchbase.Lite.Sync;

public interface ISessionService
{
    Task<AuthSession> CreateSessionAsync(Uri baseAddress, string token, CancellationToken cancellationToken = default);

    Task DeleteSessionAsync(Uri baseAddress, CancellationToken cancellationToken = default);
}

public class SessionService(IHttpClientFactory httpClientFactory) : ISessionService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private HttpClient CreateHttpClient(Uri baseAddress)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = baseAddress;
        httpClient.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse("dotnet-httpclient"));

        return httpClient;
    }

    //[Obsolete]
    //public async Task<AuthSession> CreateSessionAsync(string token, CancellationToken cancellationToken = default)
    //{
    //    var request = new HttpRequestMessage(HttpMethod.Post, "_session");
    //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

    //    var response = (await _httpClient.SendAsync(request, cancellationToken))
    //                                        .EnsureSuccessStatusCode();

    //    if (!response.Headers.TryGetValues("Set-Cookie", out var values))
    //    {
    //        throw new Exception("Session cookie not found");
    //    }

    //    var content = await response.Content.ReadFromJsonAsync<SessionContent>(cancellationToken) ?? throw new InvalidOperationException("Session content not found");
    //    return AuthSession.Create(content.UserCtx.Name, values.First());
    //}

    public async Task<AuthSession> CreateSessionAsync(Uri baseAddress, string token, CancellationToken cancellationToken = default)
    {
        var httpClient = CreateHttpClient(baseAddress);

        var request = new HttpRequestMessage(HttpMethod.Post, "_session");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = (await httpClient.SendAsync(request, cancellationToken))
                                            .EnsureSuccessStatusCode();

        if (!response.Headers.TryGetValues("Set-Cookie", out var values))
        {
            throw new Exception("Session cookie not found");
        }

        var content = await response.Content.ReadFromJsonAsync<SessionContent>(cancellationToken) ?? throw new InvalidOperationException("Session content not found");
        return AuthSession.Create(content.UserCtx.Name, values.First());
    }

    //[Obsolete]
    //public async Task DeleteSessionAsync(CancellationToken cancellationToken = default)
    //{
    //    (await _httpClient.DeleteAsync("_session", cancellationToken))
    //                        .EnsureSuccessStatusCode();
    //}

    public async Task DeleteSessionAsync(Uri baseAddress, CancellationToken cancellationToken = default)
    {
        var httpClient = CreateHttpClient(baseAddress);

        (await httpClient.DeleteAsync("_session", cancellationToken))
                            .EnsureSuccessStatusCode();
    }
}

public class AuthSession(string username, string sessionId, string path, DateTimeOffset dateTime)
{
    public static AuthSession Create(string username, string cookieValue)
    {
        var segments = cookieValue.Split(';');

        var sessionId = segments[0].Split('=').Last();
        var path = segments[1].Split('=').Last();
        var expiresOn = segments[2].Split('=').Last();

        _ = DateTimeOffset.TryParse(expiresOn, out var expiresOnDate);

        return new AuthSession(username, sessionId, path, expiresOnDate);
    }

    public string Username { get; } = username;

    public string SessionId { get; } = sessionId;

    public string Path { get; } = path;

    public DateTimeOffset ExpiresOn { get; } = dateTime;

}

public class SessionContent
{
    [JsonPropertyName("authentication_handlers")]
    public string[] AuthenticationHandlers { get; set; } = [];

    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("userCtx")]
    public Userctx UserCtx { get; set; } = new Userctx();
}

public class Userctx
{
    [JsonPropertyName("channels")]
    public object? Channels { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

