using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class LogoutTokenFactory(
    IOptions<TokenConfigurations> options,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider) : ITokenFactory
{
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<TokenResult> CreateAsync(
        ClientEntity client, 
        UserEntity? user = null, 
        SessionEntity? session = null,
        TokenFactoryOptions? factoryOptions = null, 
        CancellationToken cancellationToken = default)
    {
        if (user is null || session is null)
        {
            return TokenResult.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = "Server error"
            });
        }
        
        var lifetime = client.LogoutTokenLifetime ?? _tokenConfigurations.DefaultLogoutTokenLifetime;
        var claimsContext = new LogoutTokenClaimsContext()
        {
            Aud = client.Id.ToString(),
            Sid = session.Id.ToString(),
            Exp = DateTimeOffset.UtcNow.Add(lifetime)
        };

        var claimsFactory = _claimFactoryProvider.GetClaimFactory<LogoutTokenClaimsContext, UserEntity>();
        var claims = await claimsFactory.GetClaimsAsync(user, claimsContext, cancellationToken);
        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenTypes.Generic };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TokenResult.Success(token);
    }
}