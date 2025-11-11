using System.Net.Http.Headers;

namespace eSecurity.Client.Common.Http.Extensions;

public static class HttpRequestHeadersExtensions
{
    public static void AddBearerAuthorization(this HttpRequestHeaders headers, string? token)
    {
        headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}