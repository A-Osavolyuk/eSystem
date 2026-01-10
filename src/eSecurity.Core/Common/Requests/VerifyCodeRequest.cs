using eSecurity.Core.Security.Authorization.Access;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Core.Common.Requests;

public class VerifyCodeRequest
{
    public required Guid UserId { get; set; }
    public required SenderType Sender { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
    public required string Code { get; set; }
}