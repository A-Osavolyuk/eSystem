using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class ResetPasswordMessage : EmailMessage
{
    public string Code { get; set; } = string.Empty;
}