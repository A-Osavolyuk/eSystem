using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

[JsonConverter(typeof(JsonEnumValueConverter<UserHint>))]
public enum UserHint
{
    [EnumValue("login_hint")]
    LoginHint,
    
    [EnumValue("login_token_hint")]
    LoginTokenHint,
    
    [EnumValue("id_token_hint")]
    IdTokenHint
}