using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticatorAttachment
{
    [EnumMember(Value = "platform")]
    Platform,
    
    [EnumMember(Value = "cross-platform")]
    CrossPlatform
}