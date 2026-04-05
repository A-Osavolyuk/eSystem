using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Cryptography.Hashing;

[JsonConverter(typeof(JsonEnumValueConverter<HashAlgorithm>))]
public enum HashAlgorithm
{
    [EnumValue("pbkdf2")]
    Pbkdf2,
    
    [EnumValue("sha256")]
    Sha256,
    
    [EnumValue("sha512")]
    Sha512
}