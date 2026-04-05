using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

[JsonConverter(typeof(JsonEnumValueConverter<NotificationDeliveryMode>))]
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