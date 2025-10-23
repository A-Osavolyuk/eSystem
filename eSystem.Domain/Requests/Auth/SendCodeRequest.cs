using eSystem.Domain.Common.Messaging;
using eSystem.Domain.Security.Verification;

namespace eSystem.Domain.Requests.Auth;

public class SendCodeRequest
{
    public required Guid UserId { get; set; }
    public required SenderType Sender { get; set; }
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
    public required Dictionary<string, string> Payload { get; set; } = [];
}