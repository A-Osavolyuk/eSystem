namespace eSecurity.Server.Security.Cryptography.Tokens;

public class TokenBuilderProvider(IServiceProvider serviceProvider) : ITokenBuilderProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenBuilder<TContext, TResult> GetFactory<TContext, TResult>() where TContext : TokenBuildContext
        => _serviceProvider.GetRequiredService<ITokenBuilder<TContext, TResult>>();
}