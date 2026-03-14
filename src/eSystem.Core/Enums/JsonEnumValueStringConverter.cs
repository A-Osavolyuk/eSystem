using System.Text.Json;
using System.Text.Json.Serialization;

namespace eSystem.Core.Enums;

public sealed class JsonEnumValueStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly Dictionary<TEnum, string> To;
    private static readonly Dictionary<string, TEnum> From;

    static JsonEnumValueStringConverter()
    {
        To = new Dictionary<TEnum, string>();
        From = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);

        var enumValues = Enum.GetValues<TEnum>();
        foreach (var enumValue in enumValues)
        {
            var stringValue = EnumHelper.GetString(enumValue);
            To[enumValue] = stringValue;
            From[stringValue] = enumValue;
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}");
        
        var value = reader.GetString();
        if (!string.IsNullOrEmpty(value) && From.TryGetValue(value, out var enumValue))
            return enumValue;

        throw new JsonException($"Invalid enum value '{value}'");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        if (!To.TryGetValue(value, out var stringValue))
            throw new JsonException($"Unknown enum value '{value}'");

        writer.WriteStringValue(stringValue);
    }
}