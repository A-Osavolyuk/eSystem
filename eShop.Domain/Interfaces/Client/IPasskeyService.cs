using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IPasskeyService
{
    public ValueTask<HttpResponse> GetPasskeyAsync(Guid id);
    public ValueTask<HttpResponse> CreatePasskeyAsync(CreatePasskeyRequest request);
    public ValueTask<HttpResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request);
    public ValueTask<HttpResponse> CreateSignInOptionsAsync(PasskeySignInRequest request);
    public ValueTask<HttpResponse> VerifySignInOptionsAsync(VerifyPasskeySignInRequest request);
    public ValueTask<HttpResponse> RemovePasskeyAsync(RemovePasskeyRequest request);
}