namespace eShop.Blazor.Server.Domain.Models;

public class SecurityModel
{
    public List<UserTwoFactorMethod> Providers { get; set; } = [];
    public string QrCode { get; set; } = string.Empty;
}