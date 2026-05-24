namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenBuilderProvider
{
    public ITokenBuilder<TContext> GetFactory<TContext, TResult>() where TContext : TokenBuildContext;
}