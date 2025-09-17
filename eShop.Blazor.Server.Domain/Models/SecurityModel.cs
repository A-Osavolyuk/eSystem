namespace eShop.Blazor.Server.Domain.Models;

public class SecurityModel
{
    public List<UserProviderDto> Providers { get; set; } = [];
    public string QrCode { get; set; } = string.Empty;
}