using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public enum ResponseMode
{
    [EnumValue("query")]
    Query,
    
    [EnumValue("fragment")]
    Fragment,
    
    [EnumValue("form_post")]
    FormPost
}