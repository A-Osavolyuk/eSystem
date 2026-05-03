using System.Text.Json.Serialization;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token.Ciba;

[JsonConverter(typeof(JsonEnumValueConverter<CibaDecision>))]
public enum CibaDecision
{
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}