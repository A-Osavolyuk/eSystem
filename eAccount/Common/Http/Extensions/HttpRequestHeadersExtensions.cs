using System.Net.Http.Headers;

namespace eAccount.Common.Http.Extensions;

public static class HttpRequestHeadersExtensions
{
    public static void AddBearerAuthorization(this HttpRequestHeaders headers, string? token)
    {
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}