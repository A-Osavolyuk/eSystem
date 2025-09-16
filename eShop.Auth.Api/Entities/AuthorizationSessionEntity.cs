namespace eShop.Auth.Api.Entities;

public class AuthorizationSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }

    public UserEntity User { get; set; } = null!;
    public UserDeviceEntity Device { get; set; } = null!;
}