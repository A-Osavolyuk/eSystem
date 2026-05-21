namespace eSecurity.Idp.Security.Authorization.Authorize;

public sealed class AuthorizationFlowHandlerProvider(IServiceProvider serviceProvider) : IAuthorizationFlowHandlerProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IAuthorizationFlowHandler GetHandler(AuthorizationFlow flow)
        => _serviceProvider.GetRequiredKeyedService<IAuthorizationFlowHandler>(flow);
}