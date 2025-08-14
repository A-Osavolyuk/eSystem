namespace eShop.Domain.Requests.API.Auth;

public class UnblockDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}