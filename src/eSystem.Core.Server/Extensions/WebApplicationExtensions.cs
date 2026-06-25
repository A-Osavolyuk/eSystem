using Microsoft.AspNetCore.Builder;

namespace eSystem.Core.Server.Extensions;

public static class WebApplicationExtensions
{
    public static void MapPingEndpoint(this WebApplication app)
    {
        app.MapGet("/ping", () => "pong");
    }
}