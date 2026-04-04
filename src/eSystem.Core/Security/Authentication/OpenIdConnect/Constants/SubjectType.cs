using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

public enum SubjectType
{
    [EnumValue("public")]
    Public,
    
    [EnumValue("pairwise")]
    Pairwise
}