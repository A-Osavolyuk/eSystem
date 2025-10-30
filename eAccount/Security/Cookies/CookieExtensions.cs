namespace eAccount.Security.Cookies;

public static class CookieExtensions
{
    public static void AddCookies(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICookieAccessor, CookieAccessor>();
    }
}