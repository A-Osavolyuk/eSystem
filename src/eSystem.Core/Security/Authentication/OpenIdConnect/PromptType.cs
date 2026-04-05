using System.Text.Json.Serialization;
using eSystem.Core.Enums;

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