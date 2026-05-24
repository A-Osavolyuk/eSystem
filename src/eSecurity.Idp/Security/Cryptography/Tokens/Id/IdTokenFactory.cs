using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Identity.Claims;
using eSecurity.Idp.Security.Identity.Claims.Factories;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Id;

public sealed class IdTokenFactory(
    IOptions<TokenConfigurations> options,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory<IdTokenFactoryContext>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;

    public async ValueTask<TypedResult<string>> CreateAsync(
        IdTokenFactoryContext context,
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
        
        var subjectResult = await _subjectProvider.GetSubjectAsync(context.User, context.Client, cancellationToken);
        if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
        {
            return TypedResult<string>.Fail(new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var tokenLifetime = context.Client.IdTokenLifetime ?? _tokenConfigurations.DefaultIdTokenLifetime;
        var claimsFactory = _claimFactoryProvider.GetClaimFactory<IdTokenClaimsContext, UserEntity>();
        var authenticationMethods = context.Session.AuthenticationMethods
            .Select(x => x.MethodReference.GetString())
            .ToArray();
        
        var claims = await claimsFactory.GetClaimsAsync(context.User, new IdTokenClaimsContext
        {
            Subject = subject,
            Audience = context.Client.Id.ToString(),
            Scopes = scopes,
            SessionId = context.Session.Id.ToString(),
            AuthenticationMethods = authenticationMethods,
            AuthTime = DateTimeOffset.UtcNow,
            Expiration = DateTimeOffset.UtcNow.Add(tokenLifetime),
            Nonce = options?.Nonce
        }, cancellationToken);

        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.IdToken };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TypedResult<string>.Success(token);
    }
}