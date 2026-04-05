using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.Constants;

[JsonConverter(typeof(JsonEnumValueStringConverter<TokenKind>))]
public enum TokenKind
{
    [EnumValue("opaque")]
    Opaque,
    
    [EnumValue("jwt")]
    Jwt
}