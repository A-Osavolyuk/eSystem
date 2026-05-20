using eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class EndSessionRequestEntity : Entity
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

    public ICollection<EndSessionRequestUiLocaleEntity> UiLocales { get; set; } = null!;
}