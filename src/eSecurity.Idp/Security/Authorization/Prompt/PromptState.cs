using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authorization.Prompt;

[JsonConverter(typeof(JsonEnumValueConverter<PromptState>))]
public enum PromptState
{
    [EnumValue("success")]
    Success,
    
    [EnumValue("failed")]
    Failed,
    
    [EnumValue("next")]
    Next
}