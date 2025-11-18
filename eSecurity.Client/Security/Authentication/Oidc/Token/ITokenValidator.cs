using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public interface ITokenValidator
{
    public ValueTask<Result> ValidateAsync(string token);
}