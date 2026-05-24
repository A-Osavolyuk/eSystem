using System.Security.Claims;
using System.Text.Json;
using eSecurity.Idp.Security.Authentication.Subject;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Logout;

namespace eSecurity.Idp.Security.Cryptography.Tokens.Logout;

public sealed class LogoutTokenFactory(
    IOptions<TokenConfigurations> options,
    ITokenBuilderProvider tokenBuilderProvider,
    ISubjectProvider subjectProvider) : ITokenFactory<LogoutTokenFactoryContext>
{
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
            return TypedResult<string>.Fail(new Error
            {
                Code = ErrorCode.ServerError,
                Description = "Server error"
            });
        }
        
        var eventsJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { LogoutEvents.BackChannelLogout, new object() }
        });
        
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var exp = DateTimeOffset.UtcNow.Add(lifetime).ToUnixTimeSeconds().ToString();
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
            new(AppClaimTypes.Aud, context.Client.Id.ToString()),
            new(AppClaimTypes.Sub, subject),
            new(AppClaimTypes.Sid, context.Session.Id.ToString()),
            new(AppClaimTypes.Events, eventsJson),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
        };
        
        var tokenContext = new JwtTokenBuildContext { Claims = claims, Type = JwtTokenType.Generic };
        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var token = await tokenFactory.BuildAsync(tokenContext, cancellationToken);
        
        return TypedResult<string>.Success(token);
    }
}