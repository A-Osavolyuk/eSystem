using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class TwoFactorAuthenticationCodeMessage : EmailMessage
{
    public string Code { get; set; } = string.Empty;
}