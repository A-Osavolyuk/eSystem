using eSecurity.Core.Security.Authorization.Access;

namespace eSecurity.Core.Common.Requests;

public sealed class VerifyAuthenticatorCodeRequest
{
    public required string Subject { get; set; }
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
    public required string Code { get; set; }
}