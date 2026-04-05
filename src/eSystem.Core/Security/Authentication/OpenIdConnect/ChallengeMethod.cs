using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<ChallengeMethod>))]
public enum ChallengeMethod
{
    [EnumValue("plain")]
    Plain,
    
    [EnumValue("S256")]
    S256
}