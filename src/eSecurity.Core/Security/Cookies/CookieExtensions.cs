using Microsoft.Extensions.DependencyInjection;

namespace eSecurity.Core.Security.Cookies;

public static class CookieExtensions
{
    extension(IServiceCollection services)
    {
        public void AddCookies()
        {
            services.AddScoped<ICookieAccessor, CookieAccessor>();
        }
    }
}