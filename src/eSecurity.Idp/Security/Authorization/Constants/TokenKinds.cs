using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authorization.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<TokenKind>))]
public enum TokenKind
{
    [EnumValue("opaque")]
    Opaque,
    
    [EnumValue("jwt")]
    Jwt
}