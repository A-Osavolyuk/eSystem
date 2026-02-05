using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class AccessTokenClaimsContext : TokenClaimsContext
{
    public required IEnumerable<string> Aud { get; set; }
}

public sealed class AccessTokenClaimsFactory(IOptions<TokenOptions> options)
    : ITokenClaimsFactory<AccessTokenClaimsContext, UserEntity>, ITokenClaimsFactory<AccessTokenClaimsContext, ClientEntity>
{
    private readonly TokenOptions _options = options.Value;

    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity user,
        AccessTokenClaimsContext context, CancellationToken cancellationToken)
    {
        var exp = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime).ToUnixTimeSeconds().ToString();
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _options.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Aud)),
            new(AppClaimTypes.Sub, user.Id.ToString()),
            new(AppClaimTypes.Scope, string.Join(" ", context.Scopes)),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
        };

        if (!string.IsNullOrEmpty(context.Nonce))
            claims.Add(new Claim(AppClaimTypes.Nonce, context.Nonce));

        return ValueTask.FromResult(claims);
    }

    public ValueTask<List<Claim>> GetClaimsAsync(ClientEntity source, AccessTokenClaimsContext context,
        CancellationToken cancellationToken)
    {
        var exp = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime).ToUnixTimeSeconds().ToString();
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _options.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Aud)),
            new(AppClaimTypes.Sub, source.Id.ToString()),
            new(AppClaimTypes.Scope, string.Join(" ", context.Scopes)),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
        };

        if (!string.IsNullOrEmpty(context.Nonce))
            claims.Add(new Claim(AppClaimTypes.Nonce, context.Nonce));

        return ValueTask.FromResult(claims);
    }
}