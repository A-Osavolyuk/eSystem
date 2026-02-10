using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Devices;

public interface IDeviceService
{
    public ValueTask<ApiResponse> GetDeviceCodeInfoAsync(string userCode);
    public ValueTask<ApiResponse> CheckDeviceCodeAsync(CheckDeviceCodeRequest request);
    public ValueTask<ApiResponse> ApproveDeviceCodeAsync(ApproveDeviceCodeRequest request);
    public ValueTask<ApiResponse> DenyDeviceCodeAsync(DenyDeviceCodeRequest request);
}