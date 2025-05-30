namespace eShop.Domain.Abstraction.Messaging;

public class SmsCredentials : MessageCredentials
{
    public required string PhoneNumber { get; set; }
}