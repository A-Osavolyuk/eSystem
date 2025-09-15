using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eShop.Auth.Api.Security.Authentication.Token;

[Injectable(typeof(TokenHandler), ServiceLifetime.Scoped)]
public class TokenHandler(ICookieAccessor cookieAccessor)
{
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;
    private const string Key = "RefreshToken";

    public void Set(string token, DateTime expirationDate)
    {
        var cookieOptions = new CookieOptions()
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Expires = expirationDate,
        };

        cookieAccessor.Set(Key, token, cookieOptions);
    }

    public string? Get() => cookieAccessor.Get(Key);

    public List<Claim> Read(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var rawToken = handler.ReadJwtToken(token);
        return rawToken?.Claims.ToList() ?? [];
    }
    
    public Result Verify()
    {
        var token = cookieAccessor.Get(Key);
        if (string.IsNullOrEmpty(token)) return Results.NotFound("Token not found");

        var handler = new JwtSecurityTokenHandler();

        var rawToken = handler.ReadJwtToken(token);
        if (rawToken is null || !rawToken.Claims.Any()) return Results.BadRequest("Invalid token");

        var expClaim = rawToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
        if (expClaim is null) return Results.BadRequest("Invalid token");

        var expMilliseconds = long.Parse(expClaim.Value);
        var expDate = DateTimeOffset.FromUnixTimeMilliseconds(expMilliseconds);
        if (expDate < DateTimeOffset.UtcNow) return Results.BadRequest("Token is expired");
        
        return Result.Success();
    }
}