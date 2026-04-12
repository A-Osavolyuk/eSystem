using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<PromptType>))]
public enum PromptType
{
    [EnumValue("none")]
    None,
    
    [EnumValue("login")]
    Login,
    
    [EnumValue("consent")]
    Consent,
    
    [EnumValue("select_account")]
    SelectAccount
}