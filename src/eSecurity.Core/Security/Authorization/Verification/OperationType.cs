using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonConverter(typeof(JsonEnumValueConverter<OperationType>))]
public enum OperationType
{
    [EnumValue("change_email")]
    ChangeEmail,
    
    [EnumValue("reset_email")]
    ResetEmail,
    
    [EnumValue("remove_email")]
    RemoveEmail,
    
    [EnumValue("verify_email")]
    VerifyEmail,
    
    [EnumValue("create_software_key")]
    CreateSoftwareKey,
    
    [EnumValue("remove_software_key")]
    RemoveSoftwareKey,
    
    [EnumValue("reset_password")]
    ResetPassword,
    
    [EnumValue("enable_two_factor")]
    EnableTwoFactor,
    
    [EnumValue("disable_two_factor")]
    DisableTwoFactor,
    
    [EnumValue("show_recovery_codes")]
    ShowRecoveryCodes,
    
    [EnumValue("disable_external_account")]
    DisconnectExternalAccount,
    
    [EnumValue("reconfigure_authenticator_app")]
    ReconfigureAuthenticatorApp
}