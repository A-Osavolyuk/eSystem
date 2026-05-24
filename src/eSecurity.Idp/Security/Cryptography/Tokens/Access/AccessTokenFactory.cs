using System.Security.Claims;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Authorization.Token;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Access;

public sealed class AccessTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory<AccessTokenFactoryContext>
{
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<TypedResult<string>> CreateAsync(
        AccessTokenFactoryContext context,
        TokenFactoryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        List<string> scopes;
        if (options is null || options.AllowedScopes.Count == 0)
        {
            scopes = context.Client.AllowedScopes
                .Select(x => x.Scope.Value)
                .ToList();
        }
        else
        {
            scopes = context.Client.AllowedScopes
                .Where(x => options.AllowedScopes.Contains(x.Scope.Value))
                .Select(x => x.Scope.Value)
                .ToList();
        }

        var lifetime = context.Client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
        if (context.Client.AccessTokenType == AccessTokenType.Jwt)
        {
            var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var exp = DateTimeOffset.UtcNow.Add(lifetime).ToUnixTimeSeconds().ToString();
            
            var claims = new List<Claim>()
            {
                new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
                new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
                new(AppClaimTypes.Scope, string.Join(" ", scopes)),
                new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
                new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            };
            
            foreach (var audience in context.Client.Audiences.Select(x => x.Audience))
                claims.Add(new Claim(AppClaimTypes.Aud, audience));
            
            if (context.User is null)
            {
                claims.Add(new Claim(AppClaimTypes.ClientId, context.Client.Id.ToString()));
                claims.Add(new Claim(AppClaimTypes.Sub, context.Client.Id.ToString()));
            }
            else
            {
                var subjectResult = await _subjectProvider.GetSubjectAsync(
                    context.User, context.Client, cancellationToken);

                if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
                {
                    return TypedResult<string>.Fail(new Error
                    {
                        Code = ErrorCode.ServerError,
                        Description = "Server error"
                    });
                }
                
                claims.Add(new Claim(AppClaimTypes.Sub, subject));
                claims.Add(new Claim(AppClaimTypes.ClientId, context.User.Id.ToString()));
            }

            var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.AccessToken };
            var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();

            var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
            return TypedResult<string>.Success(token);
        }
        else
        {
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
                    return TypedResult<string>.Fail(new Error
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