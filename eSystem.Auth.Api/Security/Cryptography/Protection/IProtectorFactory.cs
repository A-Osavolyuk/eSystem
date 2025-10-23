namespace eSystem.Auth.Api.Security.Cryptography.Protection;

public interface IProtectorFactory
{
    public Protector Create(ProtectorType type);
}