using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Data.Entities;

public class SessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }

    public DateTimeOffset? ExpireDate { get; set; }
    public bool IsActive { get; set; }

    public UserDeviceEntity Device { get; set; } = null!;
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = null!;
}