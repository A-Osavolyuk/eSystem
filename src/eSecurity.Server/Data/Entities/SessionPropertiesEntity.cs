using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class SessionPropertiesEntity : Entity
{
    public Guid Id { get; set; }
    
    public DateTimeOffset? IssuedAt { get; set; }
    public DateTimeOffset? ExpiredAt { get; set; }
    
    public bool IsPersistent { get; set; }
    public bool? AllowRefresh { get; set; }
    public string? RedirectUri { get; set; }

    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;
}