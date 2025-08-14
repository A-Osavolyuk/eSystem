namespace eShop.Domain.Requests.API.Auth;

public class BlockDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}