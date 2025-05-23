using Scalar.AspNetCore;

namespace eShop.SmsSender.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApiServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.MapDefaultEndpoints();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}