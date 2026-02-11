using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Server.Data.Entities;

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

    public string? LogoUri { get; set; }
    public string? ClientUri { get; set; }

    public ICollection<ClientTokenAuthMethodEntity> TokenAuthMethods { get; set; } = null!;
    public ICollection<PairwiseSubjectEntity> PairwiseSubjects { get; set; } = null!;
    public ICollection<ClientAllowedScopeEntity> AllowedScopes { get; set; } = null!;
    public ICollection<ClientResponseTypeEntity> ResponseTypes { get; set; } = null!;
    public ICollection<ClientGrantTypeEntity> GrantTypes { get; set; } = null!;
    public ICollection<ClientAudienceEntity> Audiences { get; set; } = null!;
    public ICollection<ClientUriEntity> Uris { get; set; } = null!;

    public bool HasScopes(IEnumerable<string> scopes, out IEnumerable<string> unsupportedScopes)
    {
        unsupportedScopes = scopes.Except(AllowedScopes.Select(x => x.Scope.Value));
        return !unsupportedScopes.Any();
    }

    public bool HasScope(string scope)
        => AllowedScopes.Any(x => x.Scope.Value == scope);

    public bool HasGrantType(string grantType)
        => GrantTypes.Any(x => x.Type == grantType);

    public bool HasUri(string uri, UriType type)
        => Uris.Any(x => x.Uri == uri && x.Type == type);
    
    public bool IsValidAudience(string aud) 
        => Audiences.Any(x => x.Audience == aud);
}