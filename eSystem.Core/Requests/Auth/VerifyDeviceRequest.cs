namespace eSystem.Core.Requests.Auth;

public class VerifyDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}