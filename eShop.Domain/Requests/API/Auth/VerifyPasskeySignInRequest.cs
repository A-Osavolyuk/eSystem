using eShop.Domain.Types;

namespace eShop.Domain.Requests.API.Auth;

public class VerifyPasskeySignInRequest
{
    public required PublicKeyCredential Credential { get; set; }
}