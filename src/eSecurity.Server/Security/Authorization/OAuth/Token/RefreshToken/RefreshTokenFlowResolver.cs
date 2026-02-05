using eSecurity.Server.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class RefreshTokenFlowResolver(IServiceProvider serviceProvider) : IRefreshTokenFlowResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IRefreshTokenFlow Resolve(AuthorizationProtocol protocol)
     => _serviceProvider.GetRequiredKeyedService<IRefreshTokenFlow>(protocol);
}