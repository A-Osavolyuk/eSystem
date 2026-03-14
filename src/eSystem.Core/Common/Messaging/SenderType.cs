using eSystem.Core.Enums;

namespace eSystem.Core.Common.Messaging;

public enum SenderType
{
    [EnumValue("email")]
    Email,
    
    [EnumValue("sms")]
    Sms,
    
    [EnumValue("telegram")]
    Telegram
}