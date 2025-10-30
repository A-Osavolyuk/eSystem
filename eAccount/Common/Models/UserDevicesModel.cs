using eSystem.Core.DTOs;

namespace eAccount.Common.Models;

public class UserDevicesModel
{
    public List<UserDeviceDto> Devices { get; set; } = [];
}