namespace eSystem.Auth.Api.Security.Authentication.SSO;

public static class SsoExtensions
{
    public static void AddSSO(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IClientManager, ClientManager>();
    }
}