using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueStringConverter<SubjectType>))]
public enum SubjectType
{
    [EnumValue("public")]
    Public,
    
    [EnumValue("pairwise")]
    Pairwise
}