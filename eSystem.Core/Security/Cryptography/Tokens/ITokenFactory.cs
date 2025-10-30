using System.Security.Claims;

namespace eSystem.Core.Security.Cryptography.Tokens;

public interface ITokenFactory
{
    public string Create(IEnumerable<Claim> claims);
}