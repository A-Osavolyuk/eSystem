using eSecurity.Server.Data;
using eSecurity.Server.Security.Cors;
using eSystem.Core.Server.Data;
using eSystem.ServiceDefaults;

namespace eSecurity.Server.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapServices(this WebApplication app)
    {
        await app.ConfigureDatabaseAsync<AppDbContext>();

        app.UseRouting(); 
        app.UseCors(CorsPolicies.SpaOnly);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapReverseProxy();
        app.MapControllers();
        app.MapDefaultEndpoints();
    }
}