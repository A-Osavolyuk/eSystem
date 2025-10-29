using eSystem.Core.Security.Authorization.Access;

namespace eSystem.Core.Requests.Auth;

public class VerifyAuthenticatorCodeRequest
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}