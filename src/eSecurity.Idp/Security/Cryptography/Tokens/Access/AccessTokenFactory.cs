using System.Security.Claims;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Identity.Claims;
using eSecurity.Idp.Security.Identity.Claims.Factories;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Access;

public sealed class AccessTokenFactory(
    IOptions<TokenConfigurations> options,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory<AccessTokenFactoryContext>
{
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    
    public async ValueTask<TypedResult<string>> CreateAsync(
        AccessTokenFactoryContext context,
        TokenFactoryOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        IEnumerable<string> scopes;
        if (options is null || options.AllowedScopes.Count == 0)
        {
            scopes = context.Client.AllowedScopes.Select(x => x.Scope.Value);
        }
        else
        {
            scopes = context.Client.AllowedScopes
                .Where(x => options.AllowedScopes.Contains(x.Scope.Value))
                .Select(x => x.Scope.Value);
        }
        
        if (context.Client.AccessTokenType == AccessTokenType.Jwt)
        {
            var lifetime = context.Client.AccessTokenLifetime 
                           ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            
            IEnumerable<Claim> claims;
            if (context.User is null)
            {
                var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, ClientEntity>();
                claims = await claimsFactory.GetClaimsAsync(context.Client, new AccessTokenClaimsContext
                {
                    Expiration = DateTimeOffset.UtcNow.Add(lifetime),
                    Audiences = context.Client.Audiences.Select(x => x.Audience),
                    Scopes = scopes,
                    Subject = context.Client.Id.ToString()
                }, cancellationToken);
            }
            else
            {
                var subjectResult = await _subjectProvider.GetSubjectAsync(
                    context.User, context.Client, cancellationToken);
                
                if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
                {
                    return TypedResult<string>.Fail(new Error()
                    {
                        Code = ErrorCode.ServerError,
                        Description = "Server error"
                    });
                }
                
                var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, UserEntity>();
                claims = await claimsFactory.GetClaimsAsync(context.User, new AccessTokenClaimsContext
                {
                    Expiration = DateTimeOffset.UtcNow.Add(lifetime),
                    Audiences = context.Client.Audiences.Select(x => x.Audience),
                    Scopes = scopes,
                    Subject = subject
                }, cancellationToken);
            }

            var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.AccessToken };
            var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TypedResult<string>.Success(token);
        }
        else
        {
            var lifetime = context.Client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            OpaqueTokenBuildContext tokenContext;
            if (context.User is null)
            {
                tokenContext = new OpaqueTokenBuildContext
                {
                    TokenLength = _tokenConfigurations.OpaqueTokenLength,
                    TokenType = OpaqueTokenType.AccessToken,
                    ClientId = context.Client.Id,
                    Audiences = context.Client.Audiences.Select(x => x.Audience).ToList(),
                    Scopes = scopes.ToList(),
                    ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                    Subject = context.Client.Id.ToString(),
                    Sid = context.Session?.Id
                };
            }
            else
            {
                var subjectResult = await _subjectProvider.GetSubjectAsync(
                    context.User, context.Client, cancellationToken);
                
                if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
                {
                    return TypedResult<string>.Fail(new Error()
                    {
                        Code = ErrorCode.ServerError,
                        Description = "Server error"
                    });
                }
                
                tokenContext = new OpaqueTokenBuildContext
                {
                    TokenLength = _tokenConfigurations.OpaqueTokenLength,
                    TokenType = OpaqueTokenType.AccessToken,
                    ClientId = context.Client.Id,
                    Audiences = context.Client.Audiences.Select(x => x.Audience).ToList(),
                    Scopes = scopes.ToList(),
                    ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                    Subject = subject,
                    Sid = context.Session?.Id
                };
            }
            
            var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TypedResult<string>.Success(token);
        }
    }
}