using System.Net.Http.Headers;
using System.Text;

namespace eSecurity.Client.Common.Http.Extensions;

public static class HttpRequestHeadersExtensions
{
    extension(HttpRequestHeaders headers)
    {
        public void WithBearerAuthentication(string? token)
        {
            headers.Authorization = new AuthenticationHeaderValue(AuthenticationTypes.Bearer, token);
        }
        
        public void WithBasicAuthentication(string clientId, string clientSecret)
        {
            var bytes = Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");
            var base64 = Convert.ToBase64String(bytes);
            
            headers.Authorization = new AuthenticationHeaderValue(AuthenticationTypes.Basic, base64);
        }
        
        public void WithUserAgent(string userAgent)
        {
            headers.Add(HeaderTypes.UserAgent, userAgent);
        }
        
        public void WithCookies(string cookies)
        {
            headers.Add(HeaderTypes.Cookie, cookies);
        }

        public void WithLocale(string locale)
        {
            headers.Add(HeaderTypes.XLocale, locale);
        }
        
        public void WithTimezone(string timezone)
        {
            headers.Add(HeaderTypes.XTimezone, timezone);
        }
    }
}