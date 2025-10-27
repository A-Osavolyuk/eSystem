namespace eSystem.Core.Security.Cryptography.Protection;

public interface IProtectorFactory
{
    public IProtector Create(string purpose);
}