using eSystem.Core.DTOs;

namespace eAccount.Common.Models;

public class TrustDeviceModel
{
    public string Code { get; set; } = string.Empty;
    public UserDeviceDto Device { get; set; } = new();
}