using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class LogoutTokenClaimsContext : TokenClaimsContext
{
    public required string Sid { get; set; }
    public required string Aud { get; set; }
}

public sealed class LogoutTokenClaimsFactory(
    IOptions<TokenConfigurations> options) : ITokenClaimsFactory<LogoutTokenClaimsContext, UserEntity>
{
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity source, LogoutTokenClaimsContext context, 
        CancellationToken cancellationToken)
    {
        var eventsJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { LogoutEvents.BackChannelLogout, new object() }
        });
        
        var iat = context.Iat.HasValue 
            ? context.Iat.Value.ToUnixTimeSeconds().ToString() 
            : DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        var exp = context.Exp.ToUnixTimeSeconds().ToString();
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _tokenConfigurations.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Aud)),
            new(AppClaimTypes.Sub, source.Id.ToString()),
            new(AppClaimTypes.Sid, context.Sid),
            new(AppClaimTypes.Events, eventsJson),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
        };
        
        if (context.Nbf.HasValue)
        {
            var nbf = context.Nbf.Value.ToUnixTimeSeconds().ToString();
            claims.Add(new Claim(AppClaimTypes.Nbf, nbf, ClaimValueTypes.Integer64));
        }

        return ValueTask.FromResult(claims);
    }
}