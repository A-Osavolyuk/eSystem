namespace eShop.Auth.Api.Services;

using BCrypt.Net;

public static class PasswordHandler
{
    public static string HashPassword(string password)
    {
        var hash = BCrypt.HashPassword(password);
        return hash;
    }
}