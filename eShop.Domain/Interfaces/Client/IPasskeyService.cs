using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface IPasskeyService
{
    public ValueTask<Response> CreateAsync(CreatePasskeyRequest request);
    public ValueTask<Response> VerifyAsync(VerifyPasskeyRequest request);
    public ValueTask<Response> CreateSignInOptionsAsync(PasskeySignInRequest request);
    public ValueTask<Response> VerifySignInOptionsAsync(VerifyPasskeySignInRequest request);
}