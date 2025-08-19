using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Types;

public class PublicKeyCredentialCreationOptions
{
    [JsonPropertyName("challenge")]
    public required byte[] Challenge { get; set; }
    
    [JsonPropertyName("rp")]
    public required ReplyingParty ReplyingParty { get; set; }
    
    [JsonPropertyName("user")] 
    public required User User { get; set; }
    
    [JsonPropertyName("pubKeyCredParams")]
    public required List<PublicKeyCredentialParameter> PublicKeyCredentialParameters { get; set; }

    [JsonPropertyName("authenticatorSelection")]
    public required AuthenticatorSelection AuthenticatorSelection { get; set; }

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 60000;

    [JsonPropertyName("attestation")] 
    public Attestation Attestation { get; set; } = Attestation.None;
}

public class ReplyingParty
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("id")]
    public required string Domain { get; set; }
}

public class User
{
    [JsonPropertyName("id")]
    public required byte[] Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KeyType
{
    [EnumMember(Value = "public-key")]
    PublicKey,
    
    [EnumMember(Value = "device-public-key")]
    DevicePublicKey
}

[JsonConverter(typeof(AlgorithmConverter))]
public enum Algorithm
{
    [EnumMember(Value = "-7")]
    Es256 = -7,
    
    [EnumMember(Value = "-257")]
    Rs256 = -257
}

public class PublicKeyCredentialParameter
{
    [JsonPropertyName("type")]
    public KeyType Type { get; set; }
    
    [JsonPropertyName("alg")]
    [JsonConverter(typeof(AlgorithmConverter))]
    public Algorithm Algorithm { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticatorAttachment
{
    [EnumMember(Value = "platform")]
    Platform,
    
    [EnumMember(Value = "cross-platform")]
    CrossPlatform
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResidentKey
{
    [EnumMember(Value = "preferred")]
    Required,
    
    [EnumMember(Value = "preferred")]
    Preferred,
    
    [EnumMember(Value = "discouraged")]
    Discouraged
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserVerification
{
    [EnumMember(Value = "preferred")]
    Required,
    
    [EnumMember(Value = "preferred")]
    Preferred,
    
    [EnumMember(Value = "discouraged")]
    Discouraged
}

public class AuthenticatorSelection
{
    [JsonPropertyName("authenticatorAttachment")]
    public AuthenticatorAttachment AuthenticatorAttachment { get; set; } =  AuthenticatorAttachment.Platform;
    
    [JsonPropertyName("residentKey")]
    public ResidentKey ResidentKey { get; set; } = ResidentKey.Preferred;
    
    [JsonPropertyName("userVerification")]
    public UserVerification UserVerification { get; set; } = UserVerification.Preferred;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Attestation
{
    [EnumMember(Value = "none")]
    None,
    
    [EnumMember(Value = "direct")]
    Direct,
    
    [EnumMember(Value = "indirect")]
    Indirect
}

public class AlgorithmConverter : JsonConverter<Algorithm>
{
    public override Algorithm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => (Algorithm)reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, Algorithm value, JsonSerializerOptions options)
        => writer.WriteNumberValue((int)value);
}

