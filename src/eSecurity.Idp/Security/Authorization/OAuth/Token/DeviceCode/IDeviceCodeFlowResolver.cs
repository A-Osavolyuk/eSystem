using eSecurity.Idp.Security.Authorization.OAuth.Protocol;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.DeviceCode;

public interface IDeviceCodeFlowResolver
{
    public IDeviceCodeFlow Resolve(AuthorizationProtocol protocol);
}