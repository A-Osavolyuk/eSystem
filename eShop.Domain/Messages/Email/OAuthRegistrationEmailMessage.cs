using eShop.Domain.Abstraction.Messaging;

namespace eShop.Domain.Messages.Email;

public class OAuthRegistrationEmailMessage : EmailMessage
{
    public string TempPassword { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
}