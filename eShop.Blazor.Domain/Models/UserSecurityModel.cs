namespace eShop.Blazor.Domain.Models;

public class UserSecurityModel
{
    public Guid UserId { get; set; }
    
    public string? RecoveryEmail { get; set; } = string.Empty;
    public bool HasRecoveryEmail => !string.IsNullOrEmpty(RecoveryEmail);
    public bool RecoveryEmailConfirmed  { get; set; }
    public DateTimeOffset? RecoveryEmailChangeDate { get; set; }
    public DateTimeOffset? RecoveryEmailConfirmationDate { get; set; }
    
    public bool? TwoFactorEnabled  { get; set; }
    public List<UserProviderDto> Providers { get; set; } = [];
    public List<UserOAuthProviderDto> OAuthProviders { get; set; } = [];
    public List<UserDeviceDto> Devices { get; set; } = [];
    public List<UserPasskeyDto> Passkeys { get; set; } = [];
    
    public bool HasPassword { get; set; }
    public DateTimeOffset? PasswordChangeDate { get; set; }
    
}