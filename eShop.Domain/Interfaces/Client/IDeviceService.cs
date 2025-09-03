using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IDeviceService
{
    public ValueTask<HttpResponse> GetAsync(Guid id);
    public ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request);
    public ValueTask<HttpResponse> BlockAsync(BlockDeviceRequest request);
    public ValueTask<HttpResponse> UnblockAsync(UnblockDeviceRequest request);
    public ValueTask<HttpResponse> VerifyAsync(VerifyDeviceRequest request);
}