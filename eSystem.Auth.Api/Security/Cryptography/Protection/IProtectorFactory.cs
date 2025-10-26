using eSystem.Core.Security.Cryptography.Protection;

namespace eSystem.Auth.Api.Security.Cryptography.Protection;

public interface IProtectorFactory
{
    public IProtector Create(ProtectorType type);
}