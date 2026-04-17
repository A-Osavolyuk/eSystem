using System.Text.Json.Serialization;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth.Token.Ciba;

[JsonConverter(typeof(EnumValueConverter<CibaDecision>))]
public enum CibaDecision
{
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}