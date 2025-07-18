namespace eShop.Auth.Api.Messages.Sms;

public class ChangePhoneNumberMessage : Message
{
    public override string Build()
    {
        return $"Phone number change code: {Payload["Code"]}";
    }
}