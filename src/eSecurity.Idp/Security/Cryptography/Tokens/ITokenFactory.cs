using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenFactory<in TContext> where TContext : TokenFactoryContext
{
    public ValueTask<TypedResult<string>> CreateAsync(
        TContext context,
        TokenFactoryOptions? options = null, 
        CancellationToken cancellationToken = default);
}