using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class LogoutRequestUiLocaleEntity : Entity
{
    public Guid Id { get; set; }

    public required string Locale { get; set; }

    public Guid RequestId { get; set; }
    public LogoutRequestEntity Request { get; set; } = null!;
}