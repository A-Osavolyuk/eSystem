using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Common.Requests;

public sealed class VerifyRecoveryCodeRequest
{
    public required string Subject { get; set; }
    public required string Code { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}