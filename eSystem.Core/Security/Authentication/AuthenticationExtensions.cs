using eSystem.Core.Security.Authentication.Jwt;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Security.Authentication;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddJwtAuthentication();
    }
}