using eSecurity.Server.Security.Authorization.Token.Strategies;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using TokenContext = eSecurity.Server.Security.Authorization.Token.Strategies.TokenContext;

namespace eSecurity.Server.Security.Authorization.Token;

public class TokenContextFactory : ITokenContextFactory
{
    public TokenContext? CreateContext(TokenRequest request)
    {
        return request.GrantType switch
        {
            GrantTypes.AuthorizationCode => new AuthorizationCodeContext
            {
                ClientId = request.ClientId,
                GrantType = request.GrantType,
                CodeVerifier = request.CodeVerifier,
                RedirectUri = request.RedirectUri,
                Code = request.Code
            },
            GrantTypes.RefreshToken => new RefreshTokenContext
            {
                ClientId = request.ClientId,
                GrantType = request.GrantType,
                RefreshToken = request.RefreshToken ?? throw new NullReferenceException("Refresh token cannot be null")
            },
            GrantTypes.ClientCredentials => new ClientCredentialsContext
            {
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                GrantType = request.GrantType,
                Scope = request.Scope
            },
            _ => null
        };
    }
}