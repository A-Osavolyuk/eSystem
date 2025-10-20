namespace eShop.Auth.Api.Security.Protection;

public interface IProtectorFactory
{
    public Protector Create(ProtectorType type);
}