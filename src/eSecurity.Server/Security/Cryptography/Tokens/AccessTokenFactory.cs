using System.Security.Claims;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.Subject;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Cryptography.Tokens;

public sealed class AccessTokenFactory(
    IOptions<TokenConfigurations> options,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory
{
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    
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
        
        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            
            IEnumerable<Claim> claims;
            if (user is null)
            {
                var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, ClientEntity>();
                claims = await claimsFactory.GetClaimsAsync(client, new AccessTokenClaimsContext
                {
                    Expiration = DateTimeOffset.UtcNow.Add(lifetime),
                    Audiences = client.Audiences.Select(x => x.Audience),
                    Scopes = scopes,
                    Subject = client.Id.ToString()
                }, cancellationToken);
            }
            else
            {
                var subjectResult = await _subjectProvider.GetSubjectAsync(user, client, cancellationToken);
                if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
                {
                    return TypedResult<string>.Fail(new Error()
                    {
                        Code = ErrorTypes.OAuth.ServerError,
                        Description = "Server error"
                    });
                }
                
                var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
                claims = await claimsFactory.GetClaimsAsync(user, new AccessTokenClaimsContext
                {
                    Expiration = DateTimeOffset.UtcNow.Add(lifetime),
                    Audiences = client.Audiences.Select(x => x.Audience),
                    Scopes = scopes,
                    Subject = subject
                }, cancellationToken);
            }

            var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
            var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TypedResult<string>.Success(token);
        }
        else
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            OpaqueTokenBuildContext tokenContext;
            if (user is null)
            {
                tokenContext = new OpaqueTokenBuildContext
                {
                    TokenLength = _tokenConfigurations.OpaqueTokenLength,
                    TokenType = OpaqueTokenType.AccessToken,
                    ClientId = client.Id,
                    Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                    Scopes = scopes.ToList(),
                    ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                    Subject = client.Id.ToString(),
                    Sid = session?.Id
                };
            }
            else
            {
                var subjectResult = await _subjectProvider.GetSubjectAsync(user, client, cancellationToken);
                if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
                {
                    return TypedResult<string>.Fail(new Error()
                    {
                        Code = ErrorTypes.OAuth.ServerError,
                        Description = "Server error"
                    });
                }
                
                tokenContext = new OpaqueTokenBuildContext
                {
                    TokenLength = _tokenConfigurations.OpaqueTokenLength,
                    TokenType = OpaqueTokenType.AccessToken,
                    ClientId = client.Id,
                    Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                    Scopes = scopes.ToList(),
                    ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                    Subject = subject,
                    Sid = session?.Id
                };
            }
            
            var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TypedResult<string>.Success(token);
        }
    }
}