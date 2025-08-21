using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Domain.Interfaces.Client;

public interface ICredentialService
{
    public ValueTask<Response> CreateKeyAsync(CreatePublicKeyCredentialRequest request);
    public ValueTask<Response> VerifyKeyAsync(VerifyPublicKeyCredentialRequest request);
    public ValueTask<Response> CreateAssertionOptionsAsync(CreatePublicKeyCredentialRequestOptionsRequest request);
    public ValueTask<Response> VerifyAssertionResponseAsync(VerifyPublicKeyCredentialRequestOptionsRequest request);
}