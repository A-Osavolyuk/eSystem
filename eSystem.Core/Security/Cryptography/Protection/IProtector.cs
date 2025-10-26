namespace eSystem.Core.Security.Cryptography.Protection;

public interface IProtector
{
    public string Protect(string unprotectedText);
    public string Unprotect(string protectedText);
}