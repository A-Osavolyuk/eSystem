using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Core.Common.Requests;

public class VerifyPasskeyRequest
{
    public required Guid UserId { get; set; }
    public required PublicKeyCredential Credential { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}