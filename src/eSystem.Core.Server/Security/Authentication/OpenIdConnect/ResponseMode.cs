using eSystem.Core.Enums;

namespace eSystem.Core.Server.Security.Authentication.OpenIdConnect;

public enum ResponseMode
{
    [EnumValue("query")]
    Query,
    
    [EnumValue("fragment")]
    Fragment,
    
    [EnumValue("form_post")]
    FormPost
}