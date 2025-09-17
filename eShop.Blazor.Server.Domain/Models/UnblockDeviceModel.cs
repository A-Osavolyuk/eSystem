namespace eShop.Blazor.Server.Domain.Models;

public class UnblockDeviceModel
{
    public string Code { get; set; } = string.Empty;
    public UserDeviceDto Device { get; set; } = new();
}