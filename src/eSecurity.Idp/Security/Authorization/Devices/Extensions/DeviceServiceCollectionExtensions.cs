namespace eSecurity.Idp.Security.Authorization.Devices.Extensions;

public static class DeviceServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddDeviceManagement()
        {
            services.AddScoped<IDeviceQueryService, DeviceQueryService>();
            services.AddScoped<IDeviceCommandService, DeviceCommandService>();
        }
    }
}