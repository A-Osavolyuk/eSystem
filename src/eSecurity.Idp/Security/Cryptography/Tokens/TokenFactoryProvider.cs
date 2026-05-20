using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Cryptography.Tokens;

public sealed class TokenFactoryProvider(IServiceProvider serviceProvider) : ITokenFactoryProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenFactory GetFactory(TokenType tokenType)
        => _serviceProvider.GetRequiredKeyedService<ITokenFactory>(tokenType);
}