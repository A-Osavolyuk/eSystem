using eSystem.Core.Security.Authorization.OAuth.Token;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public interface ITokenRequestMapper
{
    public TokenRequest? Map(Dictionary<string, string> input);
}