namespace eSystem.Core.Security.Authentication.Schemes;

public static class AuthenticationSchemes
{
    public const string Basic = "Basic";
    public const string ClientSecretBasic = "ClientSecretBasic";
    public const string ClientSecretJwt = "ClientSecretJwt";
    public const string ClientSecretPost = "ClientSecretPost";
    public const string External = "External";
    public const string None = "None";
    public const string PrivateKeyJwt = "PrivateKeyJwt";
}