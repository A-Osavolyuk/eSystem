namespace eShop.Auth.Api.Entities;

public class AuthorizationSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    
    public UserDeviceEntity Device { get; set; } = null!;
}