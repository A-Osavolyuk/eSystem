namespace eShop.Blazor.Server.Domain.Models;

public class SendTwoFactorTokenModel
{
    public string Provider { get; set; } = string.Empty;
}