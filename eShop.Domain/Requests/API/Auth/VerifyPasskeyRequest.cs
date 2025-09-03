using eShop.Domain.Common.Security.Credentials;

namespace eShop.Domain.Requests.API.Auth;

public class VerifyPasskeyRequest
{
    public required Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}