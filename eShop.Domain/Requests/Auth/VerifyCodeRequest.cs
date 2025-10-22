using eShop.Domain.Common.Messaging;
using eShop.Domain.Security.Verification;

namespace eShop.Domain.Requests.Auth;

public class VerifyCodeRequest
{
    public Guid UserId { get; set; }
    public required string Code { get; set; }
    public PurposeType Purpose  { get; set; }
    public ActionType Action  { get; set; }
    public SenderType Sender { get; set; }
}