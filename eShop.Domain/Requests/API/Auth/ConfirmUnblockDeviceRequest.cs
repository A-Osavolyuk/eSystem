namespace eShop.Domain.Requests.API.Auth;

public class ConfirmUnblockDeviceRequest
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public string Code { get; set; } = string.Empty;
}