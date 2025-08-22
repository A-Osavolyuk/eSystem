using eShop.Domain.Types;

namespace eShop.Domain.Requests.API.Auth;

public class VerifyPasskeyRequest
{
    public required Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public required PublicKeyCredentialResponse Response { get; set; }
}