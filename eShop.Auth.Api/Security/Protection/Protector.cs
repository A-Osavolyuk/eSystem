namespace eShop.Auth.Api.Security.Protection;

public abstract class Protector
{
    public abstract string Protect(string unprotectedText);
    public abstract string Unprotect(string protectedText);
}