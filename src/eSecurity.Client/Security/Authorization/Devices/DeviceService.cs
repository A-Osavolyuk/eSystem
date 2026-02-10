using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Devices;

public class DeviceService(IApiClient apiClient) : IDeviceService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GetDeviceCodeInfoAsync(string userCode)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethod.Get,
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
            Method = HttpMethod.Post,
            Url = "/api/v1/Device/device-code/check",
            Data = request
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.None,
            ContentType = ContentTypes.Application.Json
        });
    }

    public async ValueTask<ApiResponse> ApproveDeviceCodeAsync(ApproveDeviceCodeRequest request)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethod.Post,
            Url = "/api/v1/Device/device-code/approve",
            Data = request
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.None,
            ContentType = ContentTypes.Application.Json
        });
    }

    public async ValueTask<ApiResponse> DenyDeviceCodeAsync(DenyDeviceCodeRequest request)
    {
        return await _apiClient.SendAsync(new ApiRequest()
        {
            Method = HttpMethod.Post,
            Url = "/api/v1/Device/device-code/deny",
            Data = request
        }, new ApiOptions()
        {
            Authentication = AuthenticationType.None,
            ContentType = ContentTypes.Application.Json
        });
    }
}