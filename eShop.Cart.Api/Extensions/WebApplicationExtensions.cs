using eShop.Cart.Api.Entities;
using eShop.Domain.Types;
using Scalar.AspNetCore;

namespace eShop.Cart.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
        app.MapGrpcService<CartServer>();
    }
}