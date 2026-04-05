using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Common.Messaging;

[JsonConverter(typeof(JsonEnumValueStringConverter<SenderType>))]
public enum SenderType
{
    [EnumValue("email")]
    Email,
    
    [EnumValue("sms")]
    Sms,
    
    [EnumValue("telegram")]
    Telegram
}