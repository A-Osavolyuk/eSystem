using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(KeyType))]
public enum KeyType
{
    [EnumValue("public-key")]
    PublicKey,
    
    [EnumValue("device-public-key")]
    DevicePublicKey
}