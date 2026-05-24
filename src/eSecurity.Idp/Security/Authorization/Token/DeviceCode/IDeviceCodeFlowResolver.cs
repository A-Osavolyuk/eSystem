namespace eSecurity.Idp.Security.Authorization.Token.DeviceCode;

public interface IDeviceCodeFlowResolver
{
    public IDeviceCodeFlow Resolve(AuthorizationProtocol protocol);
}