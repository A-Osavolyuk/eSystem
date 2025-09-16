namespace eShop.Auth.Api.Entities;

public class RefreshTokenEntity : Entity, IExpirable
{
    public Guid Id { get; init; }
    public Guid DeviceId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpireDate { get; set; }
    public UserDeviceEntity Device { get; init; } = null!;
}