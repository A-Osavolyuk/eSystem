using eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class LogoutRequestEntity : Entity
{
    public Guid Id { get; set; }

    public string? IdTokenHint { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? State { get; set; }
    public string? ClientId { get; set; }
    public string? LogoutHint { get; set; }
    
    public LogoutStatus Status { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTimeOffset? DeniedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public ICollection<LogoutRequestUiLocaleEntity> UiLocales { get; set; } = null!;
}