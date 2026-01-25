namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public static class ResponseTypes
{
    public const string Code = "code";
    
    [Obsolete("Use 'Code' instead. Removed in OAuth 2.1")]
    public const string Token = "token";
    
    [Obsolete("Use 'Code' instead. Removed in OAuth 2.1")]
    public const string IdToken = "id_token";
    
    [Obsolete("Use 'Code' instead. Removed in OAuth 2.1")]
    public const string Hybrid = "code id_token";
    
}