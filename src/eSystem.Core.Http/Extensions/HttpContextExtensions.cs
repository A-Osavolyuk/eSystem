using System.Net;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAParser;

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
            if (statusCode is >= 200 and < 300)
            {
                context.Response.StatusCode = statusCode;
                return new ObjectResult(result.Value);
            }

            if (new[] { 300, 304, 305 }.Contains(statusCode))
            {
                return new StatusCodeResult(statusCode);
            }

            if (new[] { 301, 302, 303, 307, 308 }.Contains(statusCode))
            {
                context.Response.Headers.Location = result.Uri!;
                return new StatusCodeResult(statusCode);
            }

            context.Response.StatusCode = statusCode;
            return new ObjectResult(result.GetError());
        }
    }
}