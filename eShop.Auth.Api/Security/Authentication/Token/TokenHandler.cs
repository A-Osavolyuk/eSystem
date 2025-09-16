using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eShop.Auth.Api.Security.Authentication.Token;

[Injectable(typeof(TokenHandler), ServiceLifetime.Scoped)]
public class TokenHandler(ICookieAccessor cookieAccessor)
{
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;
    private const string Key = "eAccount.Authentication.RefreshToken";

    public void Set(string token, DateTime expirationDate)
    {
        var cookieOptions = new CookieOptions()
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = false,
            Expires = expirationDate,
        };

        cookieAccessor.Set(Key, token, cookieOptions);
    }

    public string? Get() => cookieAccessor.Get(Key);

    public void Remove()
    {
        var cookieOptions = new CookieOptions()
        {
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = false,
        };
        
        cookieAccessor.Remove(Key, cookieOptions);
    }

    public List<Claim> Read(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var rawToken = handler.ReadJwtToken(token);
        return rawToken?.Claims.ToList() ?? [];
    }
    
    public Result Verify(string? token)
    {
        if (string.IsNullOrEmpty(token)) return Results.Unauthorized("Invalid token");

        var handler = new JwtSecurityTokenHandler();

        var rawToken = handler.ReadJwtToken(token);
        if (rawToken is null || !rawToken.Claims.Any()) return Results.Unauthorized("Invalid token");

        var expClaim = rawToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
        if (expClaim is null) return Results.Unauthorized("Invalid token");

        var expMilliseconds = long.Parse(expClaim.Value);
        var expDate = DateTimeOffset.FromUnixTimeSeconds(expMilliseconds);
        if (expDate < DateTimeOffset.UtcNow) return Results.Unauthorized("Invalid token");
        
        return Result.Success();
    }
}