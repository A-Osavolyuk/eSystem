using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<ChallengeMethod>))]
public enum ChallengeMethod
{
    [EnumValue("plain")]
    Plain,
    
    [EnumValue("S256")]
    S256
}