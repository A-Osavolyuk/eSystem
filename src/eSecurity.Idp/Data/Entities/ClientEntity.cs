using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Server.Data.Entities;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Idp.Data.Entities;

public class ClientEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
    public AccessTokenType AccessTokenType { get; set; }

    public bool RequireClientSecret { get; set; }
    public bool RequirePkce { get; set; }
    public string? Secret { get; set; }

    public bool AllowOfflineAccess { get; set; }
    public bool RefreshTokenRotationEnabled { get; set; } = true;
    public TimeSpan? RefreshTokenLifetime { get; set; }
    public TimeSpan? AccessTokenLifetime { get; set; }
    public TimeSpan? IdTokenLifetime { get; set; }
    public TimeSpan? LoginTokenLifetime { get; set; }
    public TimeSpan? LogoutTokenLifetime { get; set; }

    public bool AllowFrontChannelLogout { get; set; }
    public bool AllowBackChannelLogout { get; set; }

    public SubjectType SubjectType { get; set; }
    public string? SectorIdentifierUri { get; set; }
    public NotificationDeliveryMode NotificationDeliveryMode { get; set; }
}