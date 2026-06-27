using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<Algorithm>))]
public enum Algorithm
{
    [EnumValue("es256")]
    Es256 = -7,
    
    [EnumValue("rs256")]
    Rs256 = -257
}