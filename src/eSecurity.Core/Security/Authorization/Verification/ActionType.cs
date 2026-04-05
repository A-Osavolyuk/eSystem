using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonConverter(typeof(JsonEnumValueStringConverter<ActionType>))]
public enum ActionType
{
    [EnumValue("verify")]
    Verify,
    
    [EnumValue("reset")]
    Reset,
    
    [EnumValue("unlock")]
    Unlock,

    [EnumValue("trust")]
    Trust,

    [EnumValue("block")]
    Block,

    [EnumValue("unblock")]
    Unblock,

    [EnumValue("disconnect")]
    Disconnect,

    [EnumValue("allow")]
    Allow,

    [EnumValue("disallow")]
    Disallow,
    
    [EnumValue("remove")]
    Remove,
    
    [EnumValue("sign_in")]
    SignIn,
    
    [EnumValue("subscribe")]
    Subscribe,
    
    [EnumValue("unsubscribe")]
    Unsubscribe,
    
    [EnumValue("recover")]
    Recover,
    
    [EnumValue("enable")]
    Enable,
    
    [EnumValue("disable")]
    Disable,
    
    [EnumValue("manage")]
    Manage,
    
    [EnumValue("create")]
    Create,
    
    [EnumValue("change")]
    Change,
    
    [EnumValue("show")]
    Show,
    
    [EnumValue("reconfigure")]
    Reconfigure
}