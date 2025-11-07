namespace eSecurity.Middlewares;

public class InvalidEncodedRouteMiddleware() : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            var path = context.Request.Path.Value ?? string.Empty;
            
            if (path.Contains('{') || path.Contains('}') ||
                path.Contains("%7D", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("%7B", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
        }

        await next(context);
    }
}
