using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Abstraction.Messaging.Email;

namespace eShop.Domain.Messages.Email;

public class ResetPasswordEmailMessage : EmailMessage
{
    public string Code { get; set; } = string.Empty;
}