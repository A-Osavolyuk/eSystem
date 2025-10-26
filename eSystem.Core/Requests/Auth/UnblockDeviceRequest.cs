namespace eSystem.Core.Requests.Auth;

public class UnblockDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}