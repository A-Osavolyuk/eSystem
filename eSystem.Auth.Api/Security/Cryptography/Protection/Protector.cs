namespace eSystem.Auth.Api.Security.Cryptography.Protection;

public abstract class Protector
{
    public abstract string Protect(string unprotectedText);
    public abstract string Unprotect(string protectedText);
}