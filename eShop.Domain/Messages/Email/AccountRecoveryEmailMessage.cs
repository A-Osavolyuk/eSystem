using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class AccountRecoveryEmailMessage : EmailMessage
{
    public string Code { get; set; } = string.Empty;
}