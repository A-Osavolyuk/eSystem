using eSystem.Core.Http.Constants;
using eSystem.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAParser;
using RedirectResult = eSystem.Core.Primitives.RedirectResult;

namespace eSystem.Core.Http.Extensions;

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

        public IActionResult HandleResult(Result result)
        {
            var statusCode = (int)result.StatusCode;
            if (result is ValueResult valueResult)
            {
                return new ObjectResult(valueResult.Value)
                {
                    StatusCode = statusCode
                };
            }

            if (result is RedirectResult redirectResult)
            {
                if (new[] { 301, 302, 303, 307, 308 }.Contains(statusCode))
                    context.Response.Headers.Location = redirectResult.Uri;
                
                return new StatusCodeResult(statusCode);
            }

            if (result is HtmlResult contentResult)
            {
                return new ContentResult
                {
                    StatusCode = statusCode,
                    Content = contentResult.Html,
                    ContentType = ContentTypes.Text.Html
                };
            }
            
            return new ObjectResult(result.GetError())
            {
                StatusCode = statusCode
            };
        }
    }
}