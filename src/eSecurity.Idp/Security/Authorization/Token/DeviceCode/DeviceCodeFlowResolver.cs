namespace eSecurity.Idp.Security.Authorization.Token.DeviceCode;

public sealed class DeviceCodeFlowResolver(IServiceProvider serviceProvider) : IDeviceCodeFlowResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IDeviceCodeFlow Resolve(AuthorizationProtocol protocol)
        => _serviceProvider.GetRequiredKeyedService<IDeviceCodeFlow>(protocol);
}