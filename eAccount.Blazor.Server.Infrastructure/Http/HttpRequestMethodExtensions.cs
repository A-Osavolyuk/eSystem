using Microsoft.AspNetCore.Http;

namespace eAccount.Blazor.Server.Infrastructure.Http;

public static class HttpRequestMethodExtensions
{
    public static void IncludeUserAgent(this HttpRequestMessage request, HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        request.Headers.Add("User-Agent", userAgent);
    }

    public static void IncludeCookies(this HttpRequestMessage request, HttpContext context)
    {
        var cookies = context.Request.Cookies
            .ToDictionary(cookie => cookie.Key, cookie => cookie.Value)
            .Select(cookie => $"{cookie.Key}={cookie.Value}")
            .Aggregate((acc, item) => $"{acc}; {item}");
            
        request.Headers.Add("Cookie", cookies);
    }
}