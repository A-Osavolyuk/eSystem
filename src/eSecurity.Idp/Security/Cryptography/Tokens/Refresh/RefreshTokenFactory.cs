using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Authorization.Token;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Refresh;

public sealed class RefreshTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory<RefreshTokenFactoryContext>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;

    public async ValueTask<TypedResult<string>> CreateAsync(
        RefreshTokenFactoryContext context,
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
        
        var lifetime = context.Client.RefreshTokenLifetime 
                       ?? _tokenConfigurations.DefaultRefreshTokenLifetime;
        
        OpaqueTokenBuildContext tokenContext;
        if (context.User is null)
        {
            tokenContext = new OpaqueTokenBuildContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.RefreshToken,
                ClientId = context.Client.Id,
                Audiences = context.Client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = scopes.ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = context.Client.Id.ToString()
            };
        }
        else
        {
            var subjectResult = await _subjectProvider.GetSubjectAsync(context.User, context.Client, cancellationToken);
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
                TokenType = OpaqueTokenType.RefreshToken,
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