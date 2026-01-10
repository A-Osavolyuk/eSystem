namespace eSecurity.Core.Common.Requests;

public class UnblockDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}