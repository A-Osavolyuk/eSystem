using eSecurity.Core.Common.Requests;

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
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.None,
            ContentType = ContentTypes.Application.Json
        });
    }

    public async ValueTask<ApiResponse> CheckDeviceCodeAsync(CheckDeviceCodeRequest request)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethods.Post,
            Url = "/api/v1/Device/device-code/check",
            Data = request
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.None,
            ContentType = ContentTypes.Application.Json
        });
    }

    public async ValueTask<ApiResponse> SendDecisionAsync(DeviceCodeDecisionRequest request)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethods.Post,
            Url = "/api/v1/Device/device-code/decision",
            Data = request
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.Bearer,
            ContentType = ContentTypes.Application.Json
        });
    }
}