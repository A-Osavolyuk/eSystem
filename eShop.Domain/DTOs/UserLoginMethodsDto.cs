namespace eShop.Domain.DTOs;

public class UserLoginMethodsDto
{
    public PasswordData PasswordData { get; set; } = new();
    public TwoFactorData TwoFactorData { get; set; } = new();
    public LinkedAccountsData LinkedAccountsData { get; set; } = new();
    public PasskeysData PasskeysData { get; set; } = new();
}

public class PasswordData
{
    public bool HasPassword { get; set; }
    public DateTimeOffset? LastChange { get; set; }
}

public class TwoFactorData
{
    public bool Enabled { get; set; }
    public bool AuthenticatorEnabled { get; set; }
    public bool PasskeyEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public TwoFactorMethod? PreferredMethod { get; set; }
}

public class LinkedAccountsData
{
    public bool HasLinkedAccounts { get; set; }
    public List<UserLinkedAccountDto> LinkedAccounts { get; set; } = [];
}

public class PasskeysData
{
    public bool HasPasskeys { get; set; }
    public List<UserPasskeyDto> Passkeys { get; set; } = [];
}