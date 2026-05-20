using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authorization.Devices;

public class DeviceService(IApiClient apiClient) : IDeviceService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetDeviceCodeInfoAsync(string userCode)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethods.Get,
            Url = $"/api/v1/Device/device-code/{userCode}",
        });
    }

    public async ValueTask<ApiResponse> CheckDeviceCodeAsync(CheckDeviceCodeRequest request)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethods.Post,
            Url = "/api/v1/Device/device-code/check",
            Data = request
        });
    }

    public async ValueTask<ApiResponse> SendDecisionAsync(DeviceCodeDecisionRequest request)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethods.Post,
            Url = "/api/v1/Device/device-code/decision",
            Data = request
        });
    }
}