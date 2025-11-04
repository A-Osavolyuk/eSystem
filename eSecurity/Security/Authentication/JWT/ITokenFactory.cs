using System.Security.Claims;

namespace eSecurity.Security.Authentication.JWT;

public interface ITokenFactory
{
    public string Create(IEnumerable<Claim> claims);
}