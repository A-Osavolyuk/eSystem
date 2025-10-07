using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Domain.Common;

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