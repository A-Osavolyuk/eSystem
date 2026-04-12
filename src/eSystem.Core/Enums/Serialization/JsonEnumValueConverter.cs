using System.Text.Json;
using System.Text.Json.Serialization;

namespace eSystem.Core.Enums.Serialization;

public sealed class JsonEnumValueConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}");

        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
            throw new JsonException("Value cannot be null or empty");
        
        var enumValue = EnumHelper.FromString<TEnum>(value);
        return enumValue?.Value ?? throw new JsonException($"Invalid enum value '{value}'");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(EnumHelper.GetString(value));
    }
}