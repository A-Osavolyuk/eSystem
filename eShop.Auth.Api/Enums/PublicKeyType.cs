using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KeyType
{
    [EnumMember(Value = "public-key")]
    PublicKey,
    
    [EnumMember(Value = "device-public-key")]
    DevicePublicKey
}