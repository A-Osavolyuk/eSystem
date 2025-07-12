using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Abstraction.Messaging.Email;

namespace eShop.Domain.Messages.Email;

public class TwoFactorTokenEmailMessage : EmailMessage
{
    public string Token { get; set; } = string.Empty;
}