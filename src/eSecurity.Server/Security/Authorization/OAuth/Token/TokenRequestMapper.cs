using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSystem.Core.Security.Authorization.OAuth.Token.Ciba;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;
using eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public class TokenRequestMapper : ITokenRequestMapper
{
    public TokenRequest? Map(Dictionary<string, string> input)
    {
        var grantType = input.GetValueOrDefault("grant_type");
        if (string.IsNullOrWhiteSpace(grantType)) return null;
        
        return grantType switch
        {
            GrantTypes.AuthorizationCode => new AuthorizationCodeRequest()
            {
                GrantType = grantType,
                ClientId = input["client_id"],
                CodeVerifier = input.GetValueOrDefault("code_verifier"),
                RedirectUri = input.GetValueOrDefault("redirect_uri"),
                Code = input.GetValueOrDefault("code")
            },
            GrantTypes.RefreshToken => new RefreshTokenRequest()
            {
                GrantType = grantType,
                ClientId = input["client_id"],
                RefreshToken = input.GetValueOrDefault("refresh_token")
            },
            GrantTypes.ClientCredentials => new ClientCredentialsRequest()
            {
                ClientId = input["client_id"],
                GrantType = grantType,
                ClientSecret = input.GetValueOrDefault("client_secret"),
                Scope = input.GetValueOrDefault("scope")
            },
            GrantTypes.DeviceCode => new DeviceCodeRequest()
            {
                GrantType = grantType,
                ClientId = input["client_id"],
                DeviceCode = input.GetValueOrDefault("device_code")
            },
            GrantTypes.TokenExchange => new TokenExchangeRequest()
            {
                GrantType = grantType,
                ClientId = input["client_id"],
                ClientSecret = input.GetValueOrDefault("client_secret"),
                ActorToken = input.GetValueOrDefault("actor_token"),
                ActorTokenType = input.GetValueOrDefault("actor_token_type"),
                SubjectToken = input.GetValueOrDefault("subject_token"),
                SubjectTokenType = input.GetValueOrDefault("subject_token_type"),
                RequestTokenType = input.GetValueOrDefault("request_token_type"),
                Audience = input.GetValueOrDefault("audience"),
                Scope = input.GetValueOrDefault("scope")
            },
            GrantTypes.Ciba => new CibaRequest()
            {
                GrantType = grantType,
                AuthReqId = input["auth_req_id"],
                ClientId = input["client_id"],
                ClientSecret = input.GetValueOrDefault("client_secret"),
                ClientAssertion = input.GetValueOrDefault("client_assertion"),
                ClientAssertionType = input.GetValueOrDefault("client_assertion_type"),
            },
            _ => null
        };
    }
}