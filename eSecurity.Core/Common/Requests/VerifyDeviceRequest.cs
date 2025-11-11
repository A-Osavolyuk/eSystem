namespace eSecurity.Core.Common.Requests;

public class VerifyDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}