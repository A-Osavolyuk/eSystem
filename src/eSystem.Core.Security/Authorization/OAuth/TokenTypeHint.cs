using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<TokenTypeHint>))]
public enum TokenTypeHint
{
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("refresh_token")]
    RefreshToken
}