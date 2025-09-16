using UAParser;

namespace eShop.Auth.Api.Extensions;

public static class HttpContextExtensions
{
    public static string GetIpV4(this HttpContext context)
    {
        var ipV4 = context.Connection.RemoteIpAddress?.MapToIPv4().ToString()!;
        return ipV4;
    }

    public static string GetUserAgent(this HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        return userAgent;
    }
    
    public static ClientInfo GetClientInfo(this HttpContext context)
    {
        var parser = Parser.GetDefault();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var clientInfo = parser.Parse(userAgent);
        return clientInfo;
    }
}