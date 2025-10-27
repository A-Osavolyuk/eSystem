namespace eAccount.Infrastructure.Security.Authentication.JWT;

public static class TokenHandler
{
    public static JwtSecurityToken? ReadToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (string.IsNullOrEmpty(token) || !handler.CanReadToken(token))
        {
            return new JwtSecurityToken();
        }
        
        var rawToken = handler.ReadJwtToken(token);
        return rawToken;
    }
    
    public static bool Validate(string token)
    {
        var rawToken = ReadToken(token);
        
        if (rawToken is null || !rawToken.Claims.Any())
        {
            return false;
        }
        
        var isExpired = IsExpired(rawToken);

        return !isExpired;
    }
    
    private static bool IsExpired(JwtSecurityToken token)
    {
        var expValue = token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value;
        var expMilliseconds = Convert.ToInt64(expValue);
        var expData = DateTimeOffset.FromUnixTimeSeconds(expMilliseconds);

        return DateTimeOffset.Now > expData;
    }
}