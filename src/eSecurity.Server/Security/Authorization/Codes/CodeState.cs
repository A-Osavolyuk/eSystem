using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.Codes;

[JsonConverter(typeof(JsonEnumValueConverter<CodeState>))]
public enum CodeState
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("cancelled")]
    Cancelled
}