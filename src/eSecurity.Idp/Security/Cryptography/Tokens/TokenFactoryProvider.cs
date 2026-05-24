using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public sealed class TokenFactoryProvider(IServiceProvider serviceProvider) : ITokenFactoryProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenFactory<TContext> GetFactory<TContext>() where TContext : TokenFactoryContext
        => _serviceProvider.GetRequiredService<ITokenFactory<TContext>>();
}