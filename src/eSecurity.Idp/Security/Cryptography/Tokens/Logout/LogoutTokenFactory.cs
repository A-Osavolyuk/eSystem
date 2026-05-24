using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Subject;
using eSecurity.Idp.Security.Identity.Claims;
using eSecurity.Idp.Security.Identity.Claims.Factories;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Logout;

public sealed class LogoutTokenFactory(
    IOptions<TokenConfigurations> options,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory<LogoutTokenFactoryContext>
{
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly ISubjectProvider _subjectProvider = subjectProvider;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<TypedResult<string>> CreateAsync(
        LogoutTokenFactoryContext context,
        TokenFactoryOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        var lifetime = context.Client.LogoutTokenLifetime ?? _tokenConfigurations.DefaultLogoutTokenLifetime;
        var subjectResult = await _subjectProvider.GetSubjectAsync(context.User, context.Client, cancellationToken);
        if (!subjectResult.Succeeded || !subjectResult.TryGetValue(out var subject))
        {
            return TypedResult<string>.Fail(new Error()
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var claimsContext = new LogoutTokenClaimsContext()
        {
            Subject = subject,
            Audience = context.Client.Id.ToString(),
            SessionId = context.Session.Id.ToString(),
            Expiration = DateTimeOffset.UtcNow.Add(lifetime)
        };

        var claimsFactory = _claimFactoryProvider.GetClaimFactory<LogoutTokenClaimsContext, UserEntity>();
        var claims = await claimsFactory.GetClaimsAsync(context.User, claimsContext, cancellationToken);
        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.Generic };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TypedResult<string>.Success(token);
    }
}