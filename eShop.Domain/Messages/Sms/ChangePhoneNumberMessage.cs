using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Abstraction.Messaging.Sms;

namespace eShop.Domain.Messages.Sms;

public class ChangePhoneNumberSmsMessage : SmsMessage
{
    public string Code { get; set; } = string.Empty;
}