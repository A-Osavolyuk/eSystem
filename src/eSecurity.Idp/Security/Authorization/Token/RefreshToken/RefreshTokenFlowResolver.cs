namespace eSecurity.Idp.Security.Authorization.Token.RefreshToken;

public sealed class RefreshTokenFlowResolver(IServiceProvider serviceProvider) : IRefreshTokenFlowResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IRefreshTokenFlow Resolve(AuthorizationProtocol protocol)
     => _serviceProvider.GetRequiredKeyedService<IRefreshTokenFlow>(protocol);
}