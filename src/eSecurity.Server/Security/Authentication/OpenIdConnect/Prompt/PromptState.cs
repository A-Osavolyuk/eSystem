using System.Text.Json.Serialization;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt;

[JsonConverter(typeof(EnumValueConverter<PromptState>))]
public enum PromptState
{
    [EnumValue("success")]
    Success,
    
    [EnumValue("failed")]
    Failed,
    
    [EnumValue("next")]
    Next
}