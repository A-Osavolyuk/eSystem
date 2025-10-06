using eShop.Domain.Common.Security.Credentials;

namespace eShop.Domain.Requests.Auth;

public class VerifyPasskeySignInRequest
{
    public required PublicKeyCredential Credential { get; set; }
}