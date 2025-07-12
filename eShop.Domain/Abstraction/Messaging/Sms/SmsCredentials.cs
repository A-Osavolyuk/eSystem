namespace eShop.Domain.Abstraction.Messaging.Sms;

public class SmsCredentials : MessageCredentials
{
    public required string PhoneNumber { get; set; }
}