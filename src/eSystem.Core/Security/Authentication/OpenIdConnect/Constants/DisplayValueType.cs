using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

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