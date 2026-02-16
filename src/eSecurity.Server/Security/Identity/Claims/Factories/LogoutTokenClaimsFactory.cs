using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class LogoutTokenClaimsContext : TokenClaimsContext
{
    public required string SessionId { get; set; }
    public required string Audience { get; set; }
}

public sealed class LogoutTokenClaimsFactory(
    IOptions<TokenConfigurations> options) : ITokenClaimsFactory<LogoutTokenClaimsContext, UserEntity>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity user, LogoutTokenClaimsContext context, 
        CancellationToken cancellationToken)
    {
        var eventsJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { LogoutEvents.BackChannelLogout, new object() }
        });
        
        var iat = context.IssuedAt.HasValue 
            ? context.IssuedAt.Value.ToUnixTimeSeconds().ToString() 
            : DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        var exp = context.Expiration.ToUnixTimeSeconds().ToString();
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Audience)),
            new(AppClaimTypes.Sub, context.Subject),
            new(AppClaimTypes.Sid, context.SessionId),
            new(AppClaimTypes.Events, eventsJson),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
        };
        
        if (context.NotBefore.HasValue)
        {
            var nbf = context.NotBefore.Value.ToUnixTimeSeconds().ToString();
            claims.Add(new Claim(AppClaimTypes.Nbf, nbf, ClaimValueTypes.Integer64));
        }

        return ValueTask.FromResult(claims);
    }
}