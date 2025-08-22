namespace eShop.Auth.Api.Messages.Sms;

public class RemovePhoneNumberMessage : Message
{
    public override string Build()
    {
        return $"Phone number remove code: {Payload["Code"]}";
    }
}