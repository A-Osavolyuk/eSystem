using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.SSO.Client;

namespace eSystem.Auth.Api.Entities;

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
    public string? LogoUri { get; set; }
    public string? ClientUri { get; set; }

    public ICollection<ClientRedirectUriEntity> RedirectUris { get; set; } = null!;
    public ICollection<ClientAllowedScopeEntity> AllowedScopes { get; set; } = null!;
    public ICollection<ClientGrantTypeEntity> GrantTypes { get; set; } = null!;

    public bool HasUri(string uri) => RedirectUris.Any(r => r.RedirectUri == uri);

    public bool HasScopes(List<string> scopes)
        => scopes.All(scope => AllowedScopes.Any(x => x.Scope.Name == scope));
}