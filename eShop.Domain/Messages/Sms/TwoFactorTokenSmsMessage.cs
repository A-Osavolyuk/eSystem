using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Sms;

public class TwoFactorTokenSmsMessage : SmsMessage
{
    public string Token { get; set; } = string.Empty;
}