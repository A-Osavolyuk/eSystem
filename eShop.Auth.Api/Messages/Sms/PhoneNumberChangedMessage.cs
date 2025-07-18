namespace eShop.Auth.Api.Messages.Sms;

public class PhoneNumberChangedMessage : Message
{
    public override string Build()
    {
        return $"""
                Your phone number has beed changed. 
                If it was not you, please, enter code from below.
                Rollback code: {Payload["Code"]}
                """;
    }
}