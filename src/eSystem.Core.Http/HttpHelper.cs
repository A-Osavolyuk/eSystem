namespace eSystem.Core.Http;

public static class HttpHelper
{
    public static HttpMethod Map(HttpMethods httpMethod)
    {
        return httpMethod switch
        {
            HttpMethods.Get => HttpMethod.Get,
            HttpMethods.Post => HttpMethod.Post,
            HttpMethods.Put => HttpMethod.Put,
            HttpMethods.Patch => HttpMethod.Patch,
            HttpMethods.Delete => HttpMethod.Delete,
            HttpMethods.Head => HttpMethod.Head,
            HttpMethods.Options => HttpMethod.Options,
            HttpMethods.Trace => HttpMethod.Trace,
            HttpMethods.Connect => HttpMethod.Connect,
            _ => throw new NotSupportedException("Unknown HTTP method")
        };
    }
}