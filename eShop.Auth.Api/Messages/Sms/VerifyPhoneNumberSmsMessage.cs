namespace eShop.Auth.Api.Messages.Sms;

public class VerifyPhoneNumberSmsMessage : Message
{
    public override string Build()
    {
        return $"Phone number verification code: {Payload["Code"]}";
    }
}