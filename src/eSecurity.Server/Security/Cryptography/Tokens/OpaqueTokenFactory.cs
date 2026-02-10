using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Keys;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class OpaqueTokenContext : TokenContext
{
    public required Guid ClientId { get; set; }
    public required string Subject { get; set; }
    public required OpaqueTokenType TokenType { get; set; }
    public required int TokenLength { get; set; }
    public required List<string> Scopes { get; set; }
    public required List<string> Audiences { get; set; }
    public required DateTimeOffset ExpiredAt { get; set; }
    public Guid? Sid { get; set; }
    public Guid? ActorId { get; set; }
    public DateTimeOffset? NotBefore { get; set; }
    public DateTimeOffset? IssuedAt { get; set; }
}

public class OpaqueTokenFactory(
    IKeyFactory keyFactory,
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IClientManager clientManager) : ITokenFactory<OpaqueTokenContext, string>
{
    private readonly IKeyFactory _keyFactory = keyFactory;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<string> CreateTokenAsync(OpaqueTokenContext context,
        CancellationToken cancellationToken = default)
    {
        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null) throw new Exception("Client was not found");

        var token = _keyFactory.Create(context.TokenLength);
        var opaqueToken = new OpaqueTokenEntity()
        {
            Id = Guid.CreateVersion7(),
            Subject = context.Subject,
            TokenHash = _hasher.Hash(token),
            TokenType = context.TokenType,
            SessionId = context.Sid,
            ActorId = context.ActorId,
            ClientId = context.ClientId,
            NotBefore = context.NotBefore,
            ExpiredAt = context.ExpiredAt,
            IssuedAt = context.IssuedAt ?? DateTimeOffset.UtcNow,
        };

        opaqueToken.Audiences = client.Audiences
            .Where(aud => context.Audiences.Contains(aud.Audience))
            .Select(aud => new OpaqueTokenAudienceEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = opaqueToken.Id,
                AudienceId = aud.Id
            })
            .ToList();

        opaqueToken.Scopes = client.AllowedScopes
            .Where(scope => context.Scopes.Contains(scope.Scope.Value))
            .Select(scope => new OpaqueTokenScopeEntity()
            {
                Id = Guid.CreateVersion7(),
                TokenId = opaqueToken.Id,
                ScopeId = scope.Id
            })
            .ToList();

        var result = await _tokenManager.CreateAsync(opaqueToken, cancellationToken);
        if (!result.Succeeded)
        {
            var error = result.GetError();
            throw new Exception(error.Description);
        }

        return token;
    }
}