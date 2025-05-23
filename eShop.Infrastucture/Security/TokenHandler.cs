using eShop.Domain.Common.Security;

namespace eShop.Infrastructure.Security;

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
            new(AppClaimTypes.Id, token.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Id)!.Value),
            new(AppClaimTypes.Email, token.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.Email)!.Value),
            new(AppClaimTypes.UserName, token.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.UserName)!.Value),
            new(AppClaimTypes.PhoneNumber, token.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.PhoneNumber)!.Value),
        };

        claims.AddRange(token.Claims.Where(x => x.Type == AppClaimTypes.Role));
        claims.AddRange(token.Claims.Where(x => x.Type == AppClaimTypes.Permission));

        return claims;
    }
    
    public bool Validate(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        
        var rowToken = ReadToken(token);
        
        if (rowToken is null || !rowToken.Claims.Any())
        {
            return false;
        }
        
        var valid = Validate(rowToken);

        return valid;
    }
    
    private bool Validate(JwtSecurityToken token)
    {
        var expValue = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value;
        var expMilliseconds = Convert.ToInt64(expValue);
        var expData = DateTimeOffset.FromUnixTimeSeconds(expMilliseconds);

        return DateTimeOffset.Now < expData;
    }
}