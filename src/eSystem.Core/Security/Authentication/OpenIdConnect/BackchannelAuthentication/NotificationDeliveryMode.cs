using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public enum NotificationDeliveryMode
{
    [EnumValue("none")]
    None,
    
    [EnumValue("poll")]
    Poll,
    
    [EnumValue("ping")]
    Ping,
    
    [EnumValue("push")]
    Push
}