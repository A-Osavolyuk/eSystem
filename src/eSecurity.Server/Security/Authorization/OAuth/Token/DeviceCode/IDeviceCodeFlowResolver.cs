using eSecurity.Server.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public interface IDeviceCodeFlowResolver
{
    public IDeviceCodeFlow Resolve(AuthorizationProtocol protocol);
}