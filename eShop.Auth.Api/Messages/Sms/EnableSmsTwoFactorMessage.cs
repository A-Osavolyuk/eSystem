namespace eShop.Auth.Api.Messages.Sms;

public class EnableSmsTwoFactorMessage : Message
{
    public override string Build()
    {
        return $"""
                To verify 2FA with SMS, please enter 6-digit code from below.
                2FA provider verification code: {Payload["Code"]}
                """;
    }
}