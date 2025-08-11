using UAParser;

namespace eShop.Auth.Api.Utilities;

public static class RequestUtils
{
    public static string GetIpV4(HttpContext context)
    {
        var ipV4 = context.Connection.RemoteIpAddress?.MapToIPv4().ToString()!;
        return ipV4;
    }

    public static string GetUserAgent(HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        return userAgent;
    }

    public static ClientInfo GetClientInfo(HttpContext context)
    {
        var parser = Parser.GetDefault();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var clientInfo = parser.Parse(userAgent);
        return clientInfo;
    }
}