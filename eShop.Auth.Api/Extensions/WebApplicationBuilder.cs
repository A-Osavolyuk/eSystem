using Microsoft.AspNetCore.HttpOverrides;

namespace eShop.Auth.Api.Extensions;

public static class WebApplicationBuilder
{
    public static async Task MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();
        
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.MapOpenApi();
        app.MapScalarApiReference();
        await app.ConfigureDatabaseAsync<AuthDbContext>();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}