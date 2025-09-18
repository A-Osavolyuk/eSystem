namespace eShop.Auth.Api.Messages.Sms;

public class ChangePhoneNumberMessage : Message
{
    public override string Build()
    {
        return $"Phone number change code: {Payload["Code"]}";
    }

    public override void Initialize(Dictionary<string, string> payload)
    {
        Credentials["To"] = payload["To"];
        Payload = payload;
    }
}