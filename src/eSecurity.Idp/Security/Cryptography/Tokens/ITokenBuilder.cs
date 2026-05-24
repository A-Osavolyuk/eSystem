namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenBuilder<in TContext> where TContext : TokenBuildContext
{
    public ValueTask<string> BuildAsync(TContext context, CancellationToken cancellationToken = default);
}