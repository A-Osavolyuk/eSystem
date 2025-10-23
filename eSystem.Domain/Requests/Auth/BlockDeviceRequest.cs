namespace eSystem.Domain.Requests.Auth;

public class BlockDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}