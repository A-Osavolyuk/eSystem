namespace eSecurity.Server.Security.Authorization.Devices;

public static class DeviceExtensions
{
    extension(IServiceCollection services)
    {
        public void AddDeviceManagement()
        {
            services.AddScoped<IDeviceManager, DeviceManager>();
        }
    }
}