using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonConverter(typeof(JsonEnumValueConverter<PurposeType>))]
public enum PurposeType
{
    [EnumValue("email")]
    Email,
    
    [EnumValue("phone_number")]
    PhoneNumber,

    [EnumValue("account")]
    Account,

    [EnumValue("password")]
    Password,
    
    [EnumValue("device")]
    Device,

    [EnumValue("linked_account")]
    LinkedAccount,
    
    [EnumValue("passkey")]
    Passkey,
    
    [EnumValue("authenticator_app")]
    AuthenticatorApp,
    
    [EnumValue("two_factor")]
    TwoFactor,
    
    [EnumValue("login_method")]
    LoginMethod,
    
    [EnumValue("recovery_codes")]
    RecoveryCodes
}