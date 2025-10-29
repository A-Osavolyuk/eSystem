namespace eSystem.Auth.Api.Security.Authorization.Devices;

public static class DeviceExtensions
{
    public static void AddDeviceManagement(this IServiceCollection services)
    {
        services.AddScoped<IDeviceManager, DeviceManager>();
    }
}