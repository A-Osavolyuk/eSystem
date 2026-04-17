using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Devices;

public interface IDeviceService
{
    public ValueTask<ApiResponse> GetDeviceCodeInfoAsync(string userCode);
    public ValueTask<ApiResponse> CheckDeviceCodeAsync(CheckDeviceCodeRequest request);
    public ValueTask<ApiResponse> SendDecisionAsync(DeviceCodeDecisionRequest request);
}