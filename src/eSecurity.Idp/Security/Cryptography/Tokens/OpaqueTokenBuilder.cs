using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Cryptography.Hashing;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public sealed class OpaqueTokenBuildContext : TokenBuildContext
{
    public required Guid ClientId { get; set; }
    public required string Subject { get; set; }
    public required OpaqueTokenType TokenType { get; set; }
    public required int TokenLength { get; set; }
    public List<string> Scopes { get; set; } = [];
    public required DateTimeOffset ExpiredAt { get; set; }
    public List<string> Audiences { get; set; } = [];
    public Guid? Sid { get; set; }
    public Guid? ActorId { get; set; }
    public DateTimeOffset? NotBefore { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
}

public class OpaqueTokenBuilder(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IClientQueryService clientQueryService) : ITokenBuilder<OpaqueTokenBuildContext>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<string> BuildAsync(OpaqueTokenBuildContext buildContext,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientQueryService.GetByIdAsync(buildContext.ClientId, cancellationToken);
        if (client is null) throw new Exception("Client was not found");

        var token = RandomKeyFactory.Create(buildContext.TokenLength);
        var opaqueToken = new OpaqueTokenEntity
        {
            Id = Guid.CreateVersion7(),
            Subject = buildContext.Subject,
            TokenHash = _hasher.Hash(token),
            TokenType = buildContext.TokenType,
            SessionId = buildContext.Sid,
            ActorId = buildContext.ActorId,
            ClientId = buildContext.ClientId,
            NotBefore = buildContext.NotBefore,
            ExpiredAt = buildContext.ExpiredAt,
            IssuedAt = buildContext.IssuedAt ?? DateTimeOffset.UtcNow,
        };

        if (buildContext.TokenType is not OpaqueTokenType.LoginToken && buildContext.Audiences.Count > 0)
        {
            var clientAudiences = await _clientQueryService.GetSupportedAudiencesAsync(
                client, cancellationToken);
            
            opaqueToken.Audiences = clientAudiences
                .Where(aud => buildContext.Audiences.Contains(aud.Audience))
                .Select(aud => new OpaqueTokenAudienceEntity
                {
                    Id = Guid.CreateVersion7(),
                    TokenId = opaqueToken.Id,
                    AudienceId = aud.Id
                })
                .ToList();
        }

        if (buildContext.TokenType is not OpaqueTokenType.LoginToken && buildContext.Scopes.Count > 0)
        {
            var clientScopes = await _clientQueryService.GetAllowedScopesAsync(
                client, cancellationToken);
            
            opaqueToken.Scopes = clientScopes
                .Where(scope => buildContext.Scopes.Contains(scope.Scope.Value))
                .Select(scope => new OpaqueTokenScopeEntity
                {
                    Id = Guid.CreateVersion7(),
                    TokenId = opaqueToken.Id,
                    ScopeId = scope.Id
                })
                .ToList();
        }

        var result = await _tokenManager.CreateAsync(opaqueToken, cancellationToken);
        if (!result.Succeeded)
        {
            var error = result.GetError();
            throw new Exception(error.Description);
        }

        return token;
    }
}