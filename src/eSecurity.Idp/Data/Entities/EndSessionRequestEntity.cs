using eSecurity.Idp.Security.Authentication.EndSession;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class EndSessionRequestEntity : Entity
{
    public Guid Id { get; set; }
    public string? IdTokenHint { get; set; }
    public string? PostLogoutRedirectUri { get; set; }
    public string? State { get; set; }
    public string? LogoutHint { get; set; }
    
    public EndSessionStatus Status { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTimeOffset? DeniedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
    
    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;

    public ICollection<EndSessionRequestUiLocaleEntity> UiLocales { get; set; } = null!;

    public void AddUiLocales(IEnumerable<string> uiLocales)
    {
        var locales = uiLocales.ToList();
        if (locales.Count > 0)
        {
            UiLocales = [];
            foreach (var locale in locales)
            {
                UiLocales.Add(new EndSessionRequestUiLocaleEntity()
                {
                    Id = Guid.CreateVersion7(),
                    RequestId = Id,
                    Locale = locale
                });
            }
        }
    }
}