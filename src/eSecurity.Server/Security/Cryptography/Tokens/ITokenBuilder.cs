namespace eSecurity.Server.Security.Cryptography.Tokens;

public interface ITokenBuilder<in TContext, TResult> where TContext : TokenBuildContext
{
    public ValueTask<TResult> BuildAsync(TContext context, CancellationToken cancellationToken = default);
}