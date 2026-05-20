using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;

[JsonConverter(typeof(JsonEnumValueConverter<LogoutStatus>))]
public enum LogoutStatus
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied,
    
    [EnumValue("expired")]
    Expired,
    
    [EnumValue("cancelled")]
    Cancelled
}