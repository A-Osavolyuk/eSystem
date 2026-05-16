using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.Ciba;

[JsonConverter(typeof(JsonEnumValueConverter<CibaDecision>))]
public enum CibaDecision
{
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}