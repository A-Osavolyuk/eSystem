using eSystem.Core.Common.Messaging;
using eSystem.Core.Security.Authorization.Access;

namespace eSystem.Core.Requests.Auth;

public class VerifyCodeRequest
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public PurposeType Purpose  { get; set; }
    public ActionType Action  { get; set; }
    public SenderType Sender { get; set; }
}