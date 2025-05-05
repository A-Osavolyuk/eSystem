using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eShop.Domain.Interfaces.Client;

public interface ITokenProvider
{
    public ValueTask<string> GetTokenAsync();
    public ValueTask SetTokenAsync(string refreshToken);
    public ValueTask RemoveAsync();
    public JwtSecurityToken? ReadToken(string token);
    public List<Claim> ReadClaims(JwtSecurityToken token);
    public bool IsValid(JwtSecurityToken token);
}