using eSecurity.Features.Verification.Commands;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Common.Messaging;

public class MessageContext
{
    public Guid UserId { get; set; }
    public SenderType Sender { get; set; }
    public PurposeType Purpose { get; set; }
    public ActionType Action { get; set; }
    public Dictionary<string, string> Payload { get; set; } = [];

    public SendCodeCommand ToSendCodeCommand()
    {
        return new SendCodeCommand()
        {
            UserId = UserId,
            Sender = Sender,
            Purpose = Purpose,
            Action = Action,
            Payload = Payload
        };
    }
    
    public VerifyCodeCommand ToVerifyCodeCommand(string code)
    {
        return new VerifyCodeCommand()
        {
            UserId = UserId,
            Sender = Sender,
            Purpose = Purpose,
            Action = Action,
            Code = code
        };
    }
}