using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IDeviceService
{
    public ValueTask<HttpResponse> GetAsync(Guid id);
    public ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request);
    public ValueTask<HttpResponse> BlockAsync(BlockDeviceRequest request);
    public ValueTask<HttpResponse> UnblockAsync(UnblockDeviceRequest request);
    public ValueTask<HttpResponse> VerifyAsync(VerifyDeviceRequest request);
}