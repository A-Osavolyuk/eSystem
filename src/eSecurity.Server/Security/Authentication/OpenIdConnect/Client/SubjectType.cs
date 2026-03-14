using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

public enum SubjectType
{
    [EnumValue("public")]
    Public,
    
    [EnumValue("pairwise")]
    Pairwise
}