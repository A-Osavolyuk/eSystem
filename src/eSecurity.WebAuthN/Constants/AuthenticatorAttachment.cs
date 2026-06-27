using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<AuthenticatorAttachment>))]
public enum AuthenticatorAttachment
{
    [EnumValue("platform")]
    Platform,
    
    [EnumValue("cross-platform")]
    CrossPlatform
}