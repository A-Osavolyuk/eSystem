namespace eShop.Auth.Api.Messages.Sms;

public class VerifyPhoneNumberMessage : Message
{
    public override string Build()
    {
        return $"Phone number verification code: {Payload["Code"]}";
    }
}