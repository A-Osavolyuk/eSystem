using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ICredentialService
{
    public ValueTask<Response> CreateKeyAsync(CreatePasskeyRequest request);
    public ValueTask<Response> VerifyKeyAsync(VerifyPasskeyRequest request);
    public ValueTask<Response> CreateAssertionOptionsAsync(PasskeySignInRequest request);
    public ValueTask<Response> VerifyAssertionResponseAsync(VerifyPasskeySignInRequest request);
}