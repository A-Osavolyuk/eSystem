using eSystem.Core.Data.Entities;

namespace eCinema.Server.Data.Entities;

public sealed class SessionPropertiesEntity : Entity
{
    public required Guid Id { get; set; }
    public required Guid SessionId { get; set; }

    public DateTimeOffset? IssuedUtc { get; set; }
    public DateTimeOffset? ExpiresUtc { get; set; }

    public bool IsPersistent { get; set; }
    public bool? AllowRefresh { get; set; }
    public string? RedirectUri { get; set; }

    public SessionEntity Session { get; set; } = null!;
}