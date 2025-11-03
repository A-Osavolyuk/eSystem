using System.Security.Claims;

namespace eSystem.Core.Security.Authentication.JWT;

public interface IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, string secret, string algorithm);
}