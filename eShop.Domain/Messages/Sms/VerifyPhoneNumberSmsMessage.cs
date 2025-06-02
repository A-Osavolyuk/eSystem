using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Sms;

public class VerifyPhoneNumberSmsMessage : SmsMessage
{
    public string Code { get; set; } = string.Empty;
}