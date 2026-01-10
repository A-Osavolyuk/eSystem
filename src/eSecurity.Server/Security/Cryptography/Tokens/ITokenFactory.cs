namespace eSecurity.Server.Security.Cryptography.Tokens;

public interface ITokenFactory<in TContext, TResult> where TContext : TokenContext
{
    public ValueTask<TResult> CreateTokenAsync(TContext context, CancellationToken cancellationToken = default);
}