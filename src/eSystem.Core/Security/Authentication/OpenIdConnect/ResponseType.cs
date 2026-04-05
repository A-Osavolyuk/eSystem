using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<ResponseType>))]
public enum ResponseType
{
    [EnumValue("code")]
    Code
}