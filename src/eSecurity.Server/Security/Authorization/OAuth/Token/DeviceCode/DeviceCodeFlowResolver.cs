using eSecurity.Server.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class DeviceCodeFlowResolver(IServiceProvider serviceProvider) : IDeviceCodeFlowResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IDeviceCodeFlow Resolve(AuthorizationProtocol protocol)
        => _serviceProvider.GetRequiredKeyedService<IDeviceCodeFlow>(protocol);
}