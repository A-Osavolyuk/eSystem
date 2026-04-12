using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<OAuthFlow>))]
public enum OAuthFlow
{
    [EnumValue("sign_in")]
    SignIn,
    
    [EnumValue("sign_up")]
    SignUp
}