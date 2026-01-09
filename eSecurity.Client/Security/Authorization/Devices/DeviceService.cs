using eSecurity.Client.Common.Http;

namespace eSecurity.Client.Security.Authorization.Devices;

public class DeviceService(IApiClient apiClient) : IDeviceService
{
    private readonly IApiClient _apiClient = apiClient;
}