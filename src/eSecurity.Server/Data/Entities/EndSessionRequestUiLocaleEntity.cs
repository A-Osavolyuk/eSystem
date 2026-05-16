using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class EndSessionRequestUiLocaleEntity : Entity
{
    public Guid Id { get; set; }

    public required string Locale { get; set; }

    public Guid RequestId { get; set; }
    public EndSessionRequestEntity Request { get; set; } = null!;
}