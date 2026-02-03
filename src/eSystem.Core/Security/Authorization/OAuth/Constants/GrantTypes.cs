namespace eSystem.Core.Security.Authorization.OAuth.Constants;

public static class GrantTypes
{
    public const string Implicit = "implicit";
    public const string AuthorizationCode = "authorization_code";
    public const string RefreshToken = "refresh_token";
    public const string ClientCredentials = "client_credentials";
    public const string Password = "password";
    public const string DeviceCode = "urn:ietf:params:oauth:grant-type:device_code";
    public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
    public const string Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
    public const string TokenExchange = "urn:ietf:params:oauth:grant-type:token-exchange";
    public const string Ciba = "urn:openid:params:grant-type:ciba";
}