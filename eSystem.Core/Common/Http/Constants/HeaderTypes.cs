namespace eSystem.Core.Common.Http.Constants;

public static class HeaderTypes
{
    // Custom
    public const string XRetry = "X-Retry";
    
    // Authentication
    public const string Authorization = "Authorization";
    public const string WwwAuthenticate = "WWW-Authenticate";
    public const string ProxyAuthenticate = "Proxy-Authenticate";
    public const string ProxyAuthorization = "Proxy-Authorization";

    // Caching
    public const string Age = "Age";
    public const string CacheControl = "Cache-Control";
    public const string Expires = "Expires";
    public const string Pragma = "Pragma";
    public const string ETag = "ETag";
    public const string LastModified = "Last-Modified";
    public const string IfMatch = "If-Match";
    public const string IfNoneMatch = "If-None-Match";
    public const string IfModifiedSince = "If-Modified-Since";
    public const string IfUnmodifiedSince = "If-Unmodified-Since";
    public const string Vary = "Vary";

    // Conditional Requests
    public const string Range = "Range";
    public const string IfRange = "If-Range";
    public const string ContentRange = "Content-Range";

    // Connection / Control
    public const string Connection = "Connection";
    public const string KeepAlive = "Keep-Alive";
    public const string TransferEncoding = "Transfer-Encoding";
    public const string Upgrade = "Upgrade";

    // Content Metadata
    public const string ContentType = "Content-Type";
    public const string ContentLength = "Content-Length";
    public const string ContentEncoding = "Content-Encoding";
    public const string ContentLanguage = "Content-Language";
    public const string ContentLocation = "Content-Location";
    public const string ContentMd5 = "Content-MD5";
    public const string ContentDisposition = "Content-Disposition";

    // Request Metadata
    public const string Accept = "Accept";
    public const string AcceptCharset = "Accept-Charset";
    public const string AcceptEncoding = "Accept-Encoding";
    public const string AcceptLanguage = "Accept-Language";
    public const string Expect = "Expect";
    public const string Host = "Host";
    public const string UserAgent = "User-Agent";
    public const string Referer = "Referer";
    public const string Origin = "Origin";

    // Response Metadata
    public const string Server = "Server";
    public const string Date = "Date";
    public const string Location = "Location";
    public const string RetryAfter = "Retry-After";

    // Cookies
    public const string Cookie = "Cookie";
    public const string SetCookie = "Set-Cookie";

    // CORS
    public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
    public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
    public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
    public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
    public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
    public const string AccessControlMaxAge = "Access-Control-Max-Age";
    public const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
    public const string AccessControlRequestMethod = "Access-Control-Request-Method";

    // Security Headers
    public const string StrictTransportSecurity = "Strict-Transport-Security";
    public const string ContentSecurityPolicy = "Content-Security-Policy";
    public const string XContentTypeOptions = "X-Content-Type-Options";
    public const string XFrameOptions = "X-Frame-Options";
    public const string XXssProtection = "X-XSS-Protection";
    public const string ReferrerPolicy = "Referrer-Policy";
    public const string PermissionsPolicy = "Permissions-Policy";

    // WebSockets
    public const string SecWebSocketKey = "Sec-WebSocket-Key";
    public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
    public const string SecWebSocketVersion = "Sec-WebSocket-Version";
}