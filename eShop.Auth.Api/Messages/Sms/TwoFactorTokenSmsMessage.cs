namespace eShop.Auth.Api.Messages.Sms;

public class TwoFactorTokenSmsMessage : Message
{
    public override string Build()
    {
        return $"Two-factor authentication code: {Payload["Code"]}";
    }
}