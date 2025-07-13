namespace eShop.Auth.Api.Messages.Sms;

public class ResetPhoneNumberSmsMessage : Message
{
    public required string Code { get; set; }
    
    public override string Build()
    {
        return $"Phone number reset code: {Code}";
    }
}