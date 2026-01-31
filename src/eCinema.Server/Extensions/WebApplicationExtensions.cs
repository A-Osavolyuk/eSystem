using eCinema.Server.Data;
using eCinema.Server.Hubs;
using eCinema.Server.Security.Cors;
using eSystem.Core.Data;
using eSystem.ServiceDefaults;
using Scalar.AspNetCore;

namespace eCinema.Server.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task MapServicesAsync()
        {
            app.UseExceptionHandler();
            app.UseRouting();
            app.UseCors(CorsPolicies.SpaOnly);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapReverseProxy();
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapDefaultEndpoints();
            app.MapHub<AuthenticationHub>("/hubs/authentication");
            
            await app.ConfigureDatabaseAsync<AppDbContext>();
        }
    }
}