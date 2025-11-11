using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Common.Models;

public class UserDevicesModel
{
    public List<UserDeviceDto> Devices { get; set; } = [];
}