namespace eSecurity.Core.Common.Requests;

public class TrustDeviceRequest
{
    public required Guid UserId { get; set; }
    public required Guid DeviceId { get; set; }
}