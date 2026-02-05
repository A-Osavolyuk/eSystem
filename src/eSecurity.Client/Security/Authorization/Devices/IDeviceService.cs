using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Devices;

public interface IDeviceService
{
    public ValueTask<ApiResponse> CheckDeviceCodeAsync(CheckDeviceCodeRequest request);
}