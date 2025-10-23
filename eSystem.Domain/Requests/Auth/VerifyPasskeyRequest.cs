using eSystem.Domain.Security.Credentials.PublicKey;
using eSystem.Domain.Security.Verification;

namespace eSystem.Domain.Requests.Auth;

public class VerifyPasskeyRequest
{
    public required Guid UserId { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
    public required PublicKeyCredential Credential { get; set; }
}