using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IPasskeyService
{
    public ValueTask<Response> CreatePasskeyAsync(CreatePasskeyRequest request);
    public ValueTask<Response> VerifyPasskeyAsync(VerifyPasskeyRequest request);
    public ValueTask<Response> CreateSignInOptionsAsync(PasskeySignInRequest request);
    public ValueTask<Response> VerifySignInOptionsAsync(VerifyPasskeySignInRequest request);
    public ValueTask<Response> RemovePasskeyAsync(RemovePasskeyRequest request);
    public ValueTask<Response> ConfirmRemovePasskeyAsync(ConfirmRemovePasskeyRequest request);
}