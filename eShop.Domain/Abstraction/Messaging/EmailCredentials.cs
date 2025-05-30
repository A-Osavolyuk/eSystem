namespace eShop.Domain.Abstraction.Messaging;

public class EmailCredentials : MessageCredentials
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}