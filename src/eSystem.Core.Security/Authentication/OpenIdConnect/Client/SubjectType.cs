using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueConverter<SubjectType>))]
public enum SubjectType
{
    [EnumValue("public")]
    Public,
    
    [EnumValue("pairwise")]
    Pairwise
}