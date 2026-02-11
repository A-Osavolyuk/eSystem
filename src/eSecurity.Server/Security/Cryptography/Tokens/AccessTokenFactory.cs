using System.Security.Claims;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class AccessTokenFactory(
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
        
        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            
            IEnumerable<Claim> claims;
            if (user is null)
            {
                var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, ClientEntity>();
                claims = await claimsFactory.GetClaimsAsync(client, new AccessTokenClaimsContext
                {
                    Exp = DateTimeOffset.UtcNow.Add(lifetime),
                    Aud = client.Audiences.Select(x => x.Audience),
                    Scopes = scopes,
                }, cancellationToken);
            }
            else
            {
                var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
                claims = await claimsFactory.GetClaimsAsync(user, new AccessTokenClaimsContext
                {
                    Exp = DateTimeOffset.UtcNow.Add(lifetime),
                    Aud = client.Audiences.Select(x => x.Audience),
                    Scopes = scopes,
                }, cancellationToken);
            }

            var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
            var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TokenResult.Success(token);
        }
        else
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            var tokenContext = new OpaqueTokenBuildContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.AccessToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = scopes.ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = user?.Id.ToString() ?? client.Id.ToString(),
                Sid = session?.Id
            };
            
            var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TokenResult.Success(token);
        }
    }
}