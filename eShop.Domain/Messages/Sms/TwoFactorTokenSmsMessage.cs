using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Abstraction.Messaging.Sms;

namespace eShop.Domain.Messages.Sms;

public class TwoFactorTokenSmsMessage : SmsMessage
{
    public string Token { get; set; } = string.Empty;
}