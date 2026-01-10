namespace eSecurity.Server.Security.Cryptography.Tokens;

public class TokenFactoryProvider(IServiceProvider serviceProvider) : ITokenFactoryProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenFactory<TContext, TResult> GetFactory<TContext, TResult>() where TContext : TokenContext
        => _serviceProvider.GetRequiredService<ITokenFactory<TContext, TResult>>();
}