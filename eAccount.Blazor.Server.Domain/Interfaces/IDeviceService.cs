using eSystem.Domain.Common.Http;
using eSystem.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IDeviceService
{
    public ValueTask<HttpResponse> GetAsync(Guid id);
    public ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request);
    public ValueTask<HttpResponse> BlockAsync(BlockDeviceRequest request);
    public ValueTask<HttpResponse> UnblockAsync(UnblockDeviceRequest request);
    public ValueTask<HttpResponse> VerifyAsync(VerifyDeviceRequest request);
}