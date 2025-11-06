using eSecurity.Common.DTOs;

namespace eSecurity.Common.Models;

public class UserDevicesModel
{
    public List<UserDeviceDto> Devices { get; set; } = [];
}