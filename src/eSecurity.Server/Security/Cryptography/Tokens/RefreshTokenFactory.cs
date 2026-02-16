using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth.Token;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class RefreshTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider) : ITokenFactory
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;

    public async ValueTask<TypedResult<string>> CreateAsync(
        ClientEntity client, 
        UserEntity? user = null, 
        SessionEntity? session = null,
        TokenFactoryOptions? factoryOptions = null, 
        CancellationToken cancellationToken = default)
    {
        IEnumerable<string> scopes;
        if (factoryOptions is null || factoryOptions.AllowedScopes.Count == 0)
        {
            scopes = client.AllowedScopes.Select(x => x.Scope.Value);
        }
        else
        {
            scopes = client.AllowedScopes
                .Where(x => factoryOptions.AllowedScopes.Contains(x.Scope.Value))
                .Select(x => x.Scope.Value);
        }
        
        var lifetime = client.RefreshTokenLifetime ?? _tokenConfigurations.DefaultRefreshTokenLifetime;
        var tokenContext = new OpaqueTokenBuildContext
        {
            TokenLength = _tokenConfigurations.OpaqueTokenLength,
            TokenType = OpaqueTokenType.RefreshToken,
            ClientId = client.Id,
            Audiences = client.Audiences.Select(x => x.Audience).ToList(),
            Scopes = scopes.ToList(),
            ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
            Subject = user?.Id.ToString() ?? client.Id.ToString(),
            Sid = session?.Id
        };
        var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);

        return TypedResult<string>.Success(token);
    }
}