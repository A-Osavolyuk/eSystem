using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Common.Requests;

public sealed class VerifyAuthenticatorCodeRequest
{
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
    public required string Code { get; set; }
}