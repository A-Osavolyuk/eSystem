using eSystem.Core.Common.Messaging;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Verification;

namespace eAccount.Domain.Common;

public class MessageContext
{
    public Guid UserId { get; set; }
    public SenderType Sender { get; set; }
    public PurposeType Purpose { get; set; }
    public ActionType Action { get; set; }
    public Dictionary<string, string> Payload { get; set; } = [];

    public SendCodeRequest ToSendCodeRequest()
    {
        return new SendCodeRequest()
        {
            UserId = UserId,
            Sender = Sender,
            Purpose = Purpose,
            Action = Action,
            Payload = Payload
        };
    }
    
    public VerifyCodeRequest ToVerifyCodeRequest(string code)
    {
        return new VerifyCodeRequest()
        {
            UserId = UserId,
            Sender = Sender,
            Purpose = Purpose,
            Action = Action,
            Code = code
        };
    }
}