using eSystem.Domain.Common.Messaging;
using eSystem.Domain.Security.Verification;

namespace eSystem.Domain.Requests.Auth;

public class VerifyCodeRequest
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public PurposeType Purpose  { get; set; }
    public ActionType Action  { get; set; }
    public SenderType Sender { get; set; }
}