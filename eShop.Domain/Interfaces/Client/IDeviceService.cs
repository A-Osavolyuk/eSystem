using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IDeviceService
{
    public ValueTask<Response> GetAsync(Guid id);
    public ValueTask<Response> TrustAsync(TrustDeviceRequest request);
    public ValueTask<Response> BlockAsync(BlockDeviceRequest request);
    public ValueTask<Response> UnblockAsync(UnblockDeviceRequest request);
    public ValueTask<Response> VerifyAsync(VerifyDeviceRequest request);
    public ValueTask<Response> ConfirmVerifyAsync(ConfirmVerifyDeviceRequest request);
    public ValueTask<Response> ConfirmBlockAsync(ConfirmBlockDeviceRequest request);
    public ValueTask<Response> ConfirmUnblockAsync(ConfirmUnblockDeviceRequest request);
}