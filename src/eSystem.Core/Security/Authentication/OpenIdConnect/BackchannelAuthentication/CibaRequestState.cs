namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public enum CibaRequestState
{
    Pending,
    Approved,
    Denied,
    Expired,
    Consumed,
    Cancelled
}