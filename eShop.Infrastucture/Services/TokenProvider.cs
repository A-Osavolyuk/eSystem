namespace eShop.Infrastructure.Services;

using ClaimTypes = Domain.Common.Security.ClaimTypes;

public class TokenProvider(ICookieManager cookieManager) : ITokenProvider
{
    private readonly ICookieManager cookieManager = cookieManager;
    private const string Key = "access-token";

    public async ValueTask<string> GetTokenAsync()
    {
        var token = await cookieManager.GetAsync(Key);
        return token;
    }

    public async ValueTask RemoveAsync()
    {
        await cookieManager.RemoveAsync(Key);
    }

    public async ValueTask SetTokenAsync(string token)
    {
        await cookieManager.SetAsync(Key, token, 30);
    }
    
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
        var claims = token.Claims.ToList();

        var output = new List<Claim>()
        {
            new(ClaimTypes.Id, claims.FirstOrDefault(x => x.Type == ClaimTypes.Id)!.Value),
            new(ClaimTypes.UserName, claims.FirstOrDefault(x => x.Type == ClaimTypes.UserName)!.Value),
            new(ClaimTypes.Email, claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value),
            new(ClaimTypes.PhoneNumber, claims.FirstOrDefault(x => x.Type == ClaimTypes.PhoneNumber)!.Value),
        };

        return output;
    }

    public bool IsValid(JwtSecurityToken token)
    {
        var expValue = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value;
        var expMilliseconds = Convert.ToInt64(expValue);
        var expData = DateTimeOffset.FromUnixTimeSeconds(expMilliseconds);

        return DateTimeOffset.Now < expData;
    }
}