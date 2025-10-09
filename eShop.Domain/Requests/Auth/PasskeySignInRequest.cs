using eShop.Domain.Common.Security.Credentials;

namespace eShop.Domain.Requests.Auth;

public class PasskeySignInRequest
{
    public required PublicKeyCredential Credential { get; set; }
}