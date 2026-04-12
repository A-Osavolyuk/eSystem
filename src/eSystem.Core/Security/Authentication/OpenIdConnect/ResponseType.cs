using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<ResponseType>))]
public enum ResponseType
{
    [EnumValue("code")]
    Code
}