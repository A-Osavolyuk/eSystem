namespace eSystem.Core.Security.Authorization.OAuth.Constants;

public static class TokenTypes
{
    public static class Full
    {
        public const string AccessToken = "urn:ietf:params:oauth:token-type:access_token";
        public const string RefreshToken = "urn:ietf:params:oauth:token-type:refresh_token";
        public const string IdToken = "urn:ietf:params:oauth:token-type:id_token";
        public const string Jwt = "urn:ietf:params:oauth:token-type:jwt";
    }

    public static class Short
    {
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";
        public const string IdToken = "id_token";
        public const string Jwt = "jwt";
    }

    public static string ToShort(string type)
    {
        return type switch
        {
            Full.AccessToken => Short.AccessToken,
            Full.RefreshToken => Short.RefreshToken,
            Full.IdToken => Short.IdToken,
            Full.Jwt => Short.Jwt,
            _ => throw new NotSupportedException("Unsupported format")
        };
    }

    public static string ToFull(string type)
    {
        return type switch
        {
            Short.AccessToken => Full.AccessToken,
            Short.RefreshToken => Full.RefreshToken,
            Short.IdToken => Full.IdToken,
            Short.Jwt => Full.Jwt,
            _ => throw new NotSupportedException("Unsupported format")
        };
    }
}