using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Messaging;

[JsonConverter(typeof(JsonEnumValueConverter<SenderType>))]
public enum SenderType
{
    [EnumValue("email")]
    Email,
    
    [EnumValue("sms")]
    Sms,
    
    [EnumValue("telegram")]
    Telegram
}