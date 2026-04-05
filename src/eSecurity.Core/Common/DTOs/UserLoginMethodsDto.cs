using eSecurity.Core.Security.Authentication.TwoFactor;

namespace eSecurity.Core.Common.DTOs;

public class UserLoginMethodsDto
{
    [JsonPropertyName("password_data")]
    public PasswordData PasswordData { get; set; } = new();
    
    [JsonPropertyName("two_factor_data")]
    public TwoFactorData TwoFactorData { get; set; } = new();
    
    [JsonPropertyName("linked_accounts_data")]
    public LinkedAccountsData LinkedAccountsData { get; set; } = new();
    
    [JsonPropertyName("passkey_data")]
    public PasskeysData PasskeysData { get; set; } = new();
}

public class PasswordData
{
    [JsonPropertyName("has_password")]
    public bool HasPassword { get; set; }
    
    [JsonPropertyName("last_changed_at")]
    public DateTimeOffset? LastChangedAt { get; set; }
}

public class TwoFactorData
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
    
    [JsonPropertyName("authenticator_enabled")]
    public bool AuthenticatorEnabled { get; set; }
    
    [JsonPropertyName("passkey_enabled")]
    public bool PasskeyEnabled { get; set; }
    
    [JsonPropertyName("sms_enabled")]
    public bool SmsEnabled { get; set; }
    
    [JsonPropertyName("preferred_method")]
    public TwoFactorMethod? PreferredMethod { get; set; }
}

public class LinkedAccountsData
{
    [JsonPropertyName("has_linked_accounts")]
    public bool HasLinkedAccounts { get; set; }
    
    [JsonPropertyName("linked_accounts")]
    public List<UserLinkedAccountDto> LinkedAccounts { get; set; } = [];
}

public class PasskeysData
{
    [JsonPropertyName("has_passkey")]
    public bool HasPasskeys { get; set; }
    
    [JsonPropertyName("passkey")]
    public List<UserPasskeyDto> Passkeys { get; set; } = [];
}