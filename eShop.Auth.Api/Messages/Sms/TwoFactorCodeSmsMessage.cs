namespace eShop.Auth.Api.Messages.Sms;

public class TwoFactorCodeSmsMessage : Message
{
    public override string Build()
    {
        return $"2FA code: {Payload["Code"]}";
    }
}