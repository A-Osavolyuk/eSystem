using eSecurity.Security.Authentication.Odic.Client;
using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class ClientEntity : Entity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ClientType Type { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public bool RequirePkce { get; set; }
    public bool RequireClientSecret { get; set; }
    public bool AllowOfflineAccess { get; set; }
    public bool RefreshTokenRotationEnabled { get; set; } = true;
    public TimeSpan RefreshTokenLifetime { get; set; }
    public string? LogoUri { get; set; }
    public string? ClientUri { get; set; }

    public ICollection<ClientRedirectUriEntity> RedirectUris { get; set; } = null!;
    public ICollection<ClientPostLogoutRedirectUriEntity> PostLogoutRedirectUris { get; set; } = null!;
    public ICollection<ClientAllowedScopeEntity> AllowedScopes { get; set; } = null!;
    public ICollection<ClientGrantTypeEntity> GrantTypes { get; set; } = null!;

    public bool HasRedirectUri(string uri) 
        => RedirectUris.Any(r => r.Uri == uri);
    public bool HasPostLogoutRedirectUri(string uri) 
        => PostLogoutRedirectUris.Any(r => r.Uri == uri);
    public bool HasScopes(List<string> scopes)
        => scopes.All(scope => AllowedScopes.Any(x => x.Scope.Name == scope));
    public bool HasScope(string scope) 
        => AllowedScopes.Any(x => x.Scope.Name == scope);
    public bool HasGrantType(string grantType)
        => GrantTypes.Any(x => x.Type == grantType);
}