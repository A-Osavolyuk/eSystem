namespace eShop.Auth.Api.Messages.Sms;

public class ConfirmPhoneNumberChangeMessage : Message
{
    public override string Build()
    {
        return $"Phone number verification code: {Payload["Code"]}";
    }
    
    public override void Initialize(Dictionary<string, string> payload)
    {
        Credentials["To"] = payload["To"];
        Payload = payload;
    }
}