namespace eShop.Infrastructure.Security;

using ClaimTypes = Domain.Common.Security.ClaimTypes;

public class TokenHandler
{
    public JwtSecurityToken? ReadToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (string.IsNullOrEmpty(token) || !handler.CanReadToken(token))
        {
            return new JwtSecurityToken();
        }
        
        var rawToken = handler.ReadJwtToken(token);
        return rawToken;
    }

    public List<Claim> ReadClaims(JwtSecurityToken token)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Id, token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Id)!.Value),
            new(ClaimTypes.Email, token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value),
            new(ClaimTypes.UserName, token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserName)!.Value),
            new(ClaimTypes.PhoneNumber, token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PhoneNumber)!.Value),
        };

        claims.AddRange(token.Claims.Where(x => x.Type == ClaimTypes.Role));
        claims.AddRange(token.Claims.Where(x => x.Type == ClaimTypes.Permission));

        return claims;
    }

    public bool IsValid(JwtSecurityToken token)
    {
        var expValue = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value;
        var expMilliseconds = Convert.ToInt64(expValue);
        var expData = DateTimeOffset.FromUnixTimeSeconds(expMilliseconds);

        return DateTimeOffset.Now < expData;
    }
}