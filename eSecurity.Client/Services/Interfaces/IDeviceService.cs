using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Services.Interfaces;

public interface IDeviceService
{
    public ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request);
}