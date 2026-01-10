using eSystem.Core.Common.Http.Constants;
using Microsoft.AspNetCore.Http;
using UAParser;

namespace eSystem.Core.Common.Http.Context;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public string? GetLocale()
        {
            return context.Request.Headers.TryGetValue(HeaderTypes.XLocale, out var locale) 
                ? locale.ToString() 
                : null;
        }
        
        public string? GetTimeZone()
        {
            return context.Request.Headers.TryGetValue(HeaderTypes.XTimezone, out var timeZone) 
                ? timeZone.ToString() 
                : null;
        }
        
        public string GetIpV4()
        {
            var ipV4 = context.Connection.RemoteIpAddress?.MapToIPv4().ToString()!;
            return ipV4;
        }

        public string GetUserAgent()
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            return userAgent;
        }
    
        public ClientInfo GetClientInfo()
        {
            var parser = Parser.GetDefault();
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var clientInfo = parser.Parse(userAgent);
            return clientInfo;
        }
        
        public string GetCookies()
        {
            var cookies = context.Request.Cookies
                .ToDictionary(cookie => cookie.Key, cookie => cookie.Value)
                .Select(cookie => $"{cookie.Key}={cookie.Value}")
                .Aggregate((acc, item) => $"{acc}; {item}");
            
            return cookies;
        }
    }
}