using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class NewEmailVerificationMessage : EmailMessage
{
    public string Code { get; set; } = string.Empty;
}