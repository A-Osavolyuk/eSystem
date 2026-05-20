using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenFactory
{
    public ValueTask<TypedResult<string>> CreateAsync(
        ClientEntity client, 
        UserEntity? user = null, 
        SessionEntity? session = null, 
        TokenFactoryOptions? factoryOptions = null, 
        CancellationToken cancellationToken = default);
}