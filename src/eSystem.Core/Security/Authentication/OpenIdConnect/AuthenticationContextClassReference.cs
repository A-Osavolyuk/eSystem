using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<AuthenticationContextClassReference>))]
public enum AuthenticationContextClassReference
{
    [EnumValue("loa1")]
    LoA1,
    
    [EnumValue("loa2")]
    LoA2,
    
    [EnumValue("loa3")]
    LoA3
}