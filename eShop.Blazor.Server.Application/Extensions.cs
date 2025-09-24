using eShop.Blazor.Server.Application.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Blazor.Server.Application;

public static class Extensions
{
    public static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
    
    public static void AddRouting(this IHostApplicationBuilder builder, Action<RouteOptions> configureRouter)
    {
        var router = new RouteOptions();
        configureRouter(router);

        builder.Services.AddSingleton(router);
        builder.Services.AddScoped<RouteManager>();
    }
}