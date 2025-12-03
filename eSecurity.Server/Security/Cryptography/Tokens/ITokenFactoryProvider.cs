namespace eSecurity.Server.Security.Cryptography.Tokens;

public interface ITokenFactoryProvider
{
    public ITokenFactory<TContext, TResult> GetFactory<TContext, TResult>() where TContext : TokenContext;
}