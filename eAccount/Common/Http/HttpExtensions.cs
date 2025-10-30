namespace eAccount.Common.Http;

public static class HttpExtensions
{
    public static void AddHttp(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IApiClient, ApiClient>();
    }
}