using Scalar.AspNetCore;

namespace eShop.Comments.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            await app.ConfigureDatabaseAsync<AppDbContext>();
        }

        app.MapDefaultEndpoints();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
    }
}