namespace eShop.Auth.Api.Messages.Sms;

public class ResetPhoneNumberMessage : Message
{
    public override string Build()
    {
        return $"Phone number reset code: {Payload["Code"]}";
    }
}