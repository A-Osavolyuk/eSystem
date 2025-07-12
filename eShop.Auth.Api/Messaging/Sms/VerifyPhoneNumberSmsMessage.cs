namespace eShop.Auth.Api.Messaging.Sms;

public class VerifyPhoneNumberSmsMessage : Message
{
    public required string Code { get; set; }
    
    public override string Build()
    {
        return $"Phone number verification code: {Code}";
    }
}