using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class LogoutTokenClaimsContext : TokenClaimsContext
{
    public required string Sid { get; set; }
    public required string Aud { get; set; }
}

public sealed class LogoutTokenClaimsFactory(
    IOptions<TokenOptions> options) : ITokenClaimsFactory<LogoutTokenClaimsContext, UserEntity>
{
    private readonly TokenOptions _tokenOptions = options.Value;

    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity source, LogoutTokenClaimsContext context, 
        CancellationToken cancellationToken)
    {
        var events = new Dictionary<string, object>
        {
            { LogoutEvents.BackChannelLogout, new object() }
        };
        
        var eventsJson = JsonSerializer.Serialize(events);
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _tokenOptions.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Aud)),
            new(AppClaimTypes.Sub, source.Id.ToString()),
            new(AppClaimTypes.Sid, context.Sid),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Events, eventsJson),
        };

        return ValueTask.FromResult(claims);
    }
}