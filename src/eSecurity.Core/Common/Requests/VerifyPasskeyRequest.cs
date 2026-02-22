using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Core.Common.Requests;

public sealed class VerifyPasskeyRequest
{
    public required PublicKeyCredential Credential { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}