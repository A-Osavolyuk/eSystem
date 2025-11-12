using eSecurity.Server.Data;
using eSystem.Core.Data;

namespace eSecurity.Server.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task MapServicesAsync()
        {
            app.UseExceptionHandler();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapDefaultEndpoints();

            await app.ConfigureDatabaseAsync<AuthDbContext>();
        }
    }
}