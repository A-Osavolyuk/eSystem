namespace eShop.Auth.Api.Messages.Sms;

public class TwoFactorCodeSmsMessage : Message
{
    public override string Build()
    {
        return $"2FA code: {Payload["Code"]}";
    }
    
    public override void Initialize(Dictionary<string, string> payload)
    {
        Credentials["To"] = payload["To"];
        Payload = payload;
    }
}