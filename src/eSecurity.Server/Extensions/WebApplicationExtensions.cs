using eSecurity.Server.Common.Middlewares;
using eSecurity.Server.Data;
using eSystem.Core.Http.Constants;
using eSystem.Core.Data;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public async Task MapServicesAsync()
        {
            app.UseExceptionHandler();
            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                if (response.StatusCode == StatusCodes.Status405MethodNotAllowed)
                {
                    response.ContentType = ContentTypes.Application.Json;
                    var error = new Error
                    {
                        Code = ErrorCode.MethodNotAllowed,
                        Description = "Method not allowed"
                    };
                    
                    await response.WriteAsJsonAsync(error);
                }
                else if (response.StatusCode == StatusCodes.Status415UnsupportedMediaType)
                {
                    response.ContentType = ContentTypes.Application.Json;
                    var error = new Error
                    {
                        Code = ErrorCode.UnsupportedMediaType,
                        Description = "Unsupported media type"
                    };
                    
                    await response.WriteAsJsonAsync(error);
                }
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseMiddleware<RequestBufferingMiddleware>();
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