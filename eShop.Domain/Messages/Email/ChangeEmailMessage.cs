using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class ChangeEmailMessage : EmailMessage
{
    public string Code { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
}