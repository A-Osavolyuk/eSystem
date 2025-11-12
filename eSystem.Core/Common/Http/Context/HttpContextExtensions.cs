using Microsoft.AspNetCore.Http;
using UAParser;

namespace eSystem.Core.Common.Http.Context;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
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
    }
}