using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

[JsonConverter(typeof(JsonEnumValueConverter<CibaRequestState>))]
public enum CibaRequestState
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied,
    
    [EnumValue("expired")]
    Expired,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("cancelled")]
    Cancelled
}