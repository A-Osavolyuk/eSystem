using eShop.Domain.Abstraction.Messaging;
using eShop.Domain.Abstraction.Messaging.Email;

namespace eShop.Domain.Messages.Email;

public class OAuthLoginEmailMessage : EmailMessage
{
    public string TempPassword { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
}