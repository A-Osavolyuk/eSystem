namespace eAccount.Infrastructure.Http.Extensions;

public static class HttpRequestHeadersExtensions
{
    public static void AddBearerAuthorization(this HttpRequestHeaders headers, string? token)
    {
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}