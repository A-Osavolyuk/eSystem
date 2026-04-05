using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authentication.SignIn.Session;

[JsonConverter(typeof(JsonEnumValueStringConverter<SignInStatus>))]
public enum SignInStatus
{
    [EnumValue("in_progress")]
    InProgress,
    
    [EnumValue("completed")]
    Completed,
    
    [EnumValue("failed")]
    Failed,
    
    [EnumValue("expired")]
    Expired,
    
    [EnumValue("cancelled")]
    Cancelled
}