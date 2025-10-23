using eSystem.Domain.Security.Credentials.PublicKey;

namespace eSystem.Domain.Requests.Auth;

public class CreatePasskeyRequest
{
    public required Guid UserId { get; set; }
    public required string DisplayName { get; set; }
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}