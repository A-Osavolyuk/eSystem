using eSecurity.Server.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class AuthorizationCodeFlowResolver(IServiceProvider serviceProvider) : IAuthorizationCodeFlowResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IAuthorizationCodeFlow Resolve(AuthorizationProtocol protocol)
        => _serviceProvider.GetRequiredKeyedService<IAuthorizationCodeFlow>(protocol);
}