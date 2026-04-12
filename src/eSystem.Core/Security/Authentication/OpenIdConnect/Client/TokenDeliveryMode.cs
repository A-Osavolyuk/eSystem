using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueConverter<TokenDeliveryMode>))]
public enum TokenDeliveryMode
{
    [EnumValue("poll")]
    Poll,
    
    [EnumValue("ping")]
    Ping,
    
    [EnumValue("push")]
    Push
}