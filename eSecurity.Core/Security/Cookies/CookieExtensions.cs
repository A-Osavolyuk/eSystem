using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSecurity.Core.Security.Cookies;

public static class CookieExtensions
{
    public static void AddCookies(this IServiceCollection services)
    {
        services.AddScoped<ICookieAccessor, CookieAccessor>();
    }
}