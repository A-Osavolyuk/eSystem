using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class IdTokenFactory(
    IOptions<TokenConfigurations> options,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider) : ITokenFactory
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;

    public async ValueTask<TypedResult<string>> CreateAsync(
        ClientEntity client, 
        UserEntity? user = null, 
        SessionEntity? session = null, 
        TokenFactoryOptions? factoryOptions = null, 
        CancellationToken cancellationToken = default)
    {
        if (user is null || session is null)
        {
            return TypedResult<string>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
        
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
        
        var tokenLifetime = client.IdTokenLifetime ?? _tokenConfigurations.DefaultIdTokenLifetime;
        var claimsFactory = _claimFactoryProvider.GetClaimFactory<IdTokenClaimsContext, UserEntity>();
        var claims = await claimsFactory.GetClaimsAsync(user, new IdTokenClaimsContext
        {
            Aud = client.Id.ToString(),
            Scopes = scopes,
            Sid = session.Id.ToString(),
            AuthenticationMethods = session.AuthenticationMethods,
            AuthTime = DateTimeOffset.UtcNow,
            Exp = DateTimeOffset.UtcNow.Add(tokenLifetime)
        }, cancellationToken);

        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenTypes.IdToken };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TypedResult<string>.Success(token);
    }
}