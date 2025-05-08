namespace eShop.Auth.Api.Utilities;

using BCrypt.Net;

public static class PasswordHandler
{
    public static string HashPassword(string password)
    {
        return BCrypt.HashPassword(password);
    }
    
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Verify(password, hash);
    }
}