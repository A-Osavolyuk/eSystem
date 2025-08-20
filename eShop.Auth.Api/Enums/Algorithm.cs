using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Enums;

[JsonConverter(typeof(AlgorithmConverter))]
public enum Algorithm
{
    [EnumMember(Value = "-7")]
    Es256 = -7,
    
    [EnumMember(Value = "-257")]
    Rs256 = -257
}

public class AlgorithmConverter : JsonConverter<Algorithm>
{
    public override Algorithm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => (Algorithm)reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, Algorithm value, JsonSerializerOptions options)
        => writer.WriteNumberValue((int)value);
}