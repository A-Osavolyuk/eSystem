namespace eShop.Auth.Api.Entities;

public class RefreshTokenEntity : Entity, IExpirable
{
    public Guid Id { get; init; }
    public Guid DeviceId { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsValid => ExpireDate > DateTimeOffset.UtcNow;
    
    public DateTimeOffset ExpireDate { get; set; }
    public DateTimeOffset? RefreshDate { get; set; }
    public UserDeviceEntity Device { get; init; } = null!;
}