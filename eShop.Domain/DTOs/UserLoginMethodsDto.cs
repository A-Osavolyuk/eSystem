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
    public bool Enabled { get; set; }
    public bool HasPassword { get; set; }
    public DateTimeOffset? LastChange { get; set; }
}

public class TwoFactorData
{
    public bool Enabled { get; set; }
    public List<UserProviderDto> Providers { get; set; } = [];
}

public class LinkedAccountsData
{
    public bool Enabled { get; set; }
    public bool HasLinkedAccounts { get; set; }
    public List<UserOAuthProviderDto> LinkedAccounts { get; set; } = [];
}

public class PasskeysData
{
    public bool Enabled { get; set; }
    public bool HasPasskeys { get; set; }
    public List<UserPasskeyDto> Passkeys { get; set; } = [];
}