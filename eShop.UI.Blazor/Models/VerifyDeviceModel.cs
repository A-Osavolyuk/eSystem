using eShop.Domain.DTOs;

namespace eShop.BlazorWebUI.Models;

public class VerifyDeviceModel
{
    public string Code { get; set; } = string.Empty;
    public UserDeviceDto Device { get; set; } = new();
}