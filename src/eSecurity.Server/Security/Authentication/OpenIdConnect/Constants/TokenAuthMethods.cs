namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;

public static class TokenAuthMethods
{
    public const string None = "none";
    public const string PrivateKeyJwt = "private_key_jwt";
    public const string ClientSecretJwt = "client_secret_jwt";
    public const string ClientSecretPost = "client_secret_post";
    public const string ClientSecretBasic = "client_secret_basic";
}