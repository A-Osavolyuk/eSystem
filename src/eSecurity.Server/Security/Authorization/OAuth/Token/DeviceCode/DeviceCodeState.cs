namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public enum DeviceCodeState
{
    Pending,
    Approved,
    Denied,
    Consumed,
    Expired,
    Cancelled
}