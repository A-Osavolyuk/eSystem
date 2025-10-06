using eShop.Domain.Common.Security.Credentials;

namespace eShop.Domain.Requests.Auth;

public class VerifyPasskeyRequest
{
    public required Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}