namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public static class DeviceAuthorizationExtensions
{
    public static void AddDeviceAuthorization(this IServiceCollection services, 
        Action<DeviceAuthorizationOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IDeviceCodeManager, DeviceCodeManager>();
    }
}