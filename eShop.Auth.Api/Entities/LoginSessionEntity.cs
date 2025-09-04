namespace eShop.Auth.Api.Entities;

public class LoginSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    
    public LoginType Type { get; set; }
    public string? Provider { get; set; }
    
    public DateTimeOffset Timestamp { get; set; }
    public UserDeviceEntity Device { get; set; } = null!;
}