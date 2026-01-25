namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public static class ChallengeMethods
{
    public const string S256 = "S256";
    
    [Obsolete("Use 'S256' instead. 'plain' is not recommended due to security reasons.")]
    public const string Plain = "plain";
}