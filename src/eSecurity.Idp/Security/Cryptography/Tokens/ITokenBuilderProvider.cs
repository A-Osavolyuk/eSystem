namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenBuilderProvider
{
    public ITokenBuilder<TContext, TResult> GetFactory<TContext, TResult>() where TContext : TokenBuildContext;
}