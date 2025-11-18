using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Authorization.Devices;

public interface IDeviceService
{
    public ValueTask<HttpResponse<TrustDeviceResponse>> TrustAsync(TrustDeviceRequest request);
}