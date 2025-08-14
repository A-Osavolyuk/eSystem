using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IDeviceService
{
    public ValueTask<Response> TrustAsync(TrustDeviceRequest request);
    public ValueTask<Response> BlockAsync(BlockDeviceRequest request);
}