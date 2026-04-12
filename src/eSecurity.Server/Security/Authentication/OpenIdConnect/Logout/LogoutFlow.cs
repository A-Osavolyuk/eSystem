using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

[JsonConverter(typeof(JsonEnumValueConverter<LogoutFlow>))]
public enum LogoutFlow
{
    [EnumValue("frontchannel")]
    Frontchannel,
    
    [EnumValue("backchannel")]
    Backchannel
}