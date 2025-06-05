using eShop.ServiceDefaults;
using Scalar.AspNetCore;

namespace eShop.Storage.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapAppServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapDefaultEndpoints();
        app.UseExceptionHandler();

        app.Run();
    }
}