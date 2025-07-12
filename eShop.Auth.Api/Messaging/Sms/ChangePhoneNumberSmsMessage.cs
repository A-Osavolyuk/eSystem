namespace eShop.Auth.Api.Messaging.Sms;

public class ChangePhoneNumberSmsMessage : Message
{
    public required string Code { get; set; }
    
    public override string Build()
    {
        return $"Phone number change code: {Code}";
    }
}