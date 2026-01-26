namespace eSecurity.Server.Common.Middlewares;

public class RequestBufferingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.EnableBuffering();
        await next(context);
    }
}