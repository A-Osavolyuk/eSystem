using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Abstraction.Messaging.Email;

namespace eShop.Domain.Messages.Email;

public class VerifyEmailMessage : EmailMessage
{
    public string Code { get; init; } = string.Empty;
}