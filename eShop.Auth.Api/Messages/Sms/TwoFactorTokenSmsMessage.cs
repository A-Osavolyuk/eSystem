namespace eShop.Auth.Api.Messages.Sms;

public class TwoFactorTokenSmsMessage : Message
{
    public required string Token { get; set; }
    
    public override string Build()
    {
        return $"Two-factor authentication code: {Token}";
    }
}