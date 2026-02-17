using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Subject;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class AccessTokenClaimsContext : TokenClaimsContext
{
    public required IEnumerable<string> Audiences { get; set; }
    public string? ClientId { get; set; }
}

public sealed class AccessTokenClaimsFactory(IOptions<TokenConfigurations> options)
    : ITokenClaimsFactory<AccessTokenClaimsContext, UserEntity>,
        ITokenClaimsFactory<AccessTokenClaimsContext, ClientEntity>
{
    private readonly TokenConfigurations _configurations = options.Value;

    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity user,
        AccessTokenClaimsContext context, CancellationToken cancellationToken)
    {
        var iat = context.IssuedAt.HasValue
            ? context.IssuedAt.Value.ToUnixTimeSeconds().ToString()
            : DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var exp = context.Expiration.ToUnixTimeSeconds().ToString();
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _configurations.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Audiences.ToArray())),
            new(AppClaimTypes.Sub, context.Subject),
            new(AppClaimTypes.Scope, string.Join(" ", context.Scopes)),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
        };

        if (context.NotBefore.HasValue)
        {
            var nbf = context.NotBefore.Value.ToUnixTimeSeconds().ToString();
            claims.Add(new Claim(AppClaimTypes.Nbf, nbf, ClaimValueTypes.Integer64));
        }

        if (string.IsNullOrEmpty(context.ClientId))
            claims.Add(new Claim(AppClaimTypes.ClientId, user.Id.ToString()));

        return ValueTask.FromResult(claims);
    }

    public ValueTask<List<Claim>> GetClaimsAsync(ClientEntity client, AccessTokenClaimsContext context,
        CancellationToken cancellationToken)
    {
        var iat = context.IssuedAt.HasValue
            ? context.IssuedAt.Value.ToUnixTimeSeconds().ToString()
            : DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var exp = context.Expiration.ToUnixTimeSeconds().ToString();
        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _configurations.Issuer),
            new(AppClaimTypes.Aud, JsonSerializer.Serialize(context.Audiences)),
            new(AppClaimTypes.ClientId, client.Id.ToString()),
            new(AppClaimTypes.Sub, context.Subject),
            new(AppClaimTypes.Scope, string.Join(" ", context.Scopes)),
            new(AppClaimTypes.Exp, exp, ClaimValueTypes.Integer64),
            new(AppClaimTypes.Iat, iat, ClaimValueTypes.Integer64),
        };

        if (context.NotBefore.HasValue)
        {
            var nbf = context.NotBefore.Value.ToUnixTimeSeconds().ToString();
            claims.Add(new Claim(AppClaimTypes.Nbf, nbf, ClaimValueTypes.Integer64));
        }

        return ValueTask.FromResult(claims);
    }
}