using eSystem.ServiceDefaults;
using Scalar.AspNetCore;

namespace eCinema.Server.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public void MapServices()
        {
            app.UseExceptionHandler();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapDefaultEndpoints();
        }
    }
}