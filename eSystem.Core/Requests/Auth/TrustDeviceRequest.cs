namespace eSystem.Core.Requests.Auth;

public class TrustDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}