using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace eSystem.Core.Common.Json.Converters;

public class JsonStringEnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
{
    private readonly Type _enumType = typeof(T);

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        foreach (var field in _enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
            if (enumMember?.Value == stringValue)
                return (T)field.GetValue(null)!;
        }
        
        return Enum.TryParse<T>(stringValue, ignoreCase: true, out var value) 
            ? value 
            : throw new JsonException($"Unknown value '{stringValue}' for enum '{_enumType.Name}'.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var field = _enumType.GetField(value.ToString()!)!;
        var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
        writer.WriteStringValue(enumMember?.Value ?? value.ToString());
    }
}