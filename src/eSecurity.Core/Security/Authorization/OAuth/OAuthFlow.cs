using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<OAuthFlow>))]
public enum OAuthFlow
{
    [EnumValue("sign_in")]
    SignIn,
    
    [EnumValue("sign_up")]
    SignUp
}