using eSystem.Core.Common.Messaging;
using eSystem.Core.Security.Verification;

namespace eSystem.Core.Requests.Auth;

public class ResendCodeRequest
{
    public required Guid UserId { get; set; }
    public required SenderType Sender { get; set; }
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
    public required Dictionary<string, string> Payload { get; set; }
}