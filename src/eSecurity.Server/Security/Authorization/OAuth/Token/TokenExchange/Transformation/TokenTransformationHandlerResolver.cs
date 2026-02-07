using eSecurity.Server.Security.Authorization.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public sealed class TokenTransformationHandlerResolver(IServiceProvider serviceProvider) : ITokenTransformationHandlerResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenTransformationHandler Resolve(TokenKind kind)
        => _serviceProvider.GetRequiredKeyedService<ITokenTransformationHandler>(kind);
}