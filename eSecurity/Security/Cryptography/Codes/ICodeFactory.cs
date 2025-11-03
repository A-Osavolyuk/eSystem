namespace eSecurity.Security.Cryptography.Codes;

public interface ICodeFactory
{
    public string Create(uint length = 6);
}