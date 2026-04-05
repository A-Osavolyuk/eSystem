using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

[JsonConverter(typeof(JsonEnumValueConverter<LogoutFlow>))]
public enum LogoutFlow
{
    [EnumValue("frontchannel")]
    Frontchannel,
    
    [EnumValue("backchannel")]
    Backchannel
}