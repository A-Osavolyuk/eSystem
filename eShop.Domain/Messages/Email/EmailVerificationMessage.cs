using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class VerifyEmailMessage : EmailMessage
{
    public string Code { get; init; } = string.Empty;
}