using eShop.Domain.Security.Credentials.PublicKey;
using eShop.Domain.Security.Verification;

namespace eShop.Domain.Requests.Auth;

public class VerifyPasskeyRequest
{
    public required Guid UserId { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
    public required PublicKeyCredential Credential { get; set; }
}