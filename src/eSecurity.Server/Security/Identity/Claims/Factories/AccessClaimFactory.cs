using System.Security.Claims;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Identity.Claims.Factories;

public sealed class AccessClaimsContext : ClaimsContext
{

}

public sealed class AccessClaimFactory(IOptions<TokenOptions> options) : IClaimFactory<AccessClaimsContext>
{
    private readonly TokenOptions _options = options.Value;

    public ValueTask<List<Claim>> GetClaimsAsync(UserEntity user,
        AccessClaimsContext context, CancellationToken cancellationToken)
    {
        var exp = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime).ToUnixTimeSeconds().ToString();
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var claims = new List<Claim>
        {
            new(AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new(AppClaimTypes.Iss, _options.Issuer),
            new(AppClaimTypes.Aud, context.Aud),
            new(AppClaimTypes.Sub, user.Id.ToString()),
            new(AppClaimTypes.Scope, string.Join(" ", context.Scopes)),
            new(AppClaimTypes.Sid, context.Sid),
            new(AppClaimTypes.Exp, exp),
            new(AppClaimTypes.Iat, iat),
        };

        if (!string.IsNullOrEmpty(context.Nonce)) 
            claims.Add(new Claim(AppClaimTypes.Nonce, context.Nonce));

        return ValueTask.FromResult(claims);
    }
}