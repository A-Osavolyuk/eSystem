namespace eSystem.Auth.Api.Messages.Sms;

public class VerificationSmsMessage : Message
{
    public override string Build()
    {
        return $"Your verification code {Payload["Code"]}";
    }

    public override void Initialize(Dictionary<string, string> payload)
    {
        Credentials["To"] = payload["To"];
        Payload = payload;
    }
}