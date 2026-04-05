using System.Text.Json;
using System.Text.Json.Serialization;

namespace eSystem.Core.Enums;

public sealed class JsonUnpreferredEnumValueConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}");

        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
            throw new JsonException("Value cannot be null or empty");
        
        var enumValue = EnumHelper.FromString<TEnum>(value);
        if (enumValue is null)
            throw new JsonException($"Invalid enum value '{value}'");
        
        return enumValue.IsPreferred 
            ? throw new JsonException("Only unpreferred value is allowed.") 
            : enumValue.Value;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        var stringValue = EnumHelper.GetString(value, false);
        if (string.IsNullOrEmpty(stringValue))
            throw new JsonException("Only unpreferred token type is allowed.");
        
        writer.WriteStringValue(stringValue);
    }
}