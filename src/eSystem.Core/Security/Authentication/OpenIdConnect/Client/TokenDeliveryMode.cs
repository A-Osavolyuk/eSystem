using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

public enum TokenDeliveryMode
{
    [EnumValue("poll")]
    Poll,
    
    [EnumValue("ping")]
    Ping,
    
    [EnumValue("push")]
    Push
}