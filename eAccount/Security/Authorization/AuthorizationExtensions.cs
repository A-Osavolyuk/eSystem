using eAccount.Security.Authorization.Access;
using eAccount.Security.Authorization.Devices;
using eAccount.Security.Authorization.OAuth;

namespace eAccount.Security.Authorization;

public static class AuthorizationExtensions
{
    public static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<AuthorizationManager>();
        builder.Services.AddScoped<ConfirmationManager>();
        builder.Services.AddScoped<IOAuthService, OAuthService>();
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddScoped<IVerificationService, VerificationService>();
    }
}