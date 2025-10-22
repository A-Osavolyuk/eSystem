namespace eShop.Domain.Security.Authentication;

public enum LoginType
{
    Password,
    TwoFactor,
    OAuth,
    Passkey
}