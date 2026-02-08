using eSecurity.Server.Security.Authorization.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public sealed class TokenDelegationHandlerResolver(IServiceProvider serviceProvider) : ITokenDelegationHandlerResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenDelegationHandler Resolve(TokenKind kind)
        => _serviceProvider.GetRequiredKeyedService<ITokenDelegationHandler>(kind);
}