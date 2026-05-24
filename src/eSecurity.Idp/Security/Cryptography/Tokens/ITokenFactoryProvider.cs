namespace eSecurity.Idp.Security.Cryptography.Tokens;

public interface ITokenFactoryProvider
{
    public ITokenFactory<TContext> GetFactory<TContext>() 
        where TContext : TokenFactoryContext;
}