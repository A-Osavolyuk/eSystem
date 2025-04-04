using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Sms;

public class ChangePhoneNumberMessage : SmsMessage
{
    public string Code { get; set; } = string.Empty;
}