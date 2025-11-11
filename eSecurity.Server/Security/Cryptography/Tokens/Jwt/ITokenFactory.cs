using System.Security.Claims;

namespace eSecurity.Server.Security.Cryptography.Tokens.Jwt;

public interface ITokenFactory
{
    public Task<string> CreateAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
}