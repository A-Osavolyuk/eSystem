using System.Net.Http.Headers;
using System.Text;
using eSystem.Core.Common.Http.Constants;

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
    }
}