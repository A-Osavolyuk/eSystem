using eSystem.Core.Enums;

namespace eSystem.Core.Server.Security.Authentication.OpenIdConnect;

public enum DisplayValueType
{
    [EnumValue("page")]
    Page,
    
    [EnumValue("popup")]
    Popup,
    
    [EnumValue("touch")]
    Touch,
    
    [EnumValue("wap")]
    Wap
}