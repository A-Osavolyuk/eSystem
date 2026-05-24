using eSecurity.Idp.Security.Authorization.Constants;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange.Delegation;

public sealed class TokenDelegationHandlerResolver(IServiceProvider serviceProvider) : ITokenDelegationHandlerResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenDelegationHandler Resolve(TokenKind kind)
        => _serviceProvider.GetRequiredKeyedService<ITokenDelegationHandler>(kind);
}