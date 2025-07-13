namespace eShop.Auth.Api.Messages.Sms;

public class ChangePhoneNumberSmsMessage : Message
{
    public override string Build()
    {
        return $"Phone number change code: {Payload["Code"]}";
    }
}