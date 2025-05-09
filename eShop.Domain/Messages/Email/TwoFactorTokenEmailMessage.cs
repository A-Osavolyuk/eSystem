using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class TwoFactorTokenEmailMessage : EmailMessage
{
    public string Token { get; set; } = string.Empty;
}