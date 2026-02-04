using eSecurity.Server.Security.Authorization.Protocol;

namespace eSecurity.Server.Security.Authorization.Token.DeviceCode;

public interface IDeviceCodeFlowResolver
{
    public IDeviceCodeFlow Resolve(AuthorizationProtocol protocol);
}