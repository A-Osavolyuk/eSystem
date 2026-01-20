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
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapReverseProxy();
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapDefaultEndpoints();
        }
    }
}