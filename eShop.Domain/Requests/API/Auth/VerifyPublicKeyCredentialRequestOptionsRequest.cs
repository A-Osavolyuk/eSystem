using eShop.Domain.Types;

namespace eShop.Domain.Requests.API.Auth;

public class VerifyPublicKeyCredentialRequestOptionsRequest
{
    public required PublicKeyCredential Credential { get; set; }
}