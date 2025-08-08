namespace eShop.Domain.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    public DateTimeOffset? UserNameChangeDate { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset? EmailChangeDate { get; set; }
    
    public string? RecoveryEmail { get; set; } = string.Empty;
    public DateTimeOffset? RecoveryEmailChangeDate { get; set; }
    
    public string? PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    
    public bool TwoFactorEnabled { get; set; }
    public List<UserProviderDto> Providers { get; set; } = [];

    public bool HasLinkedAccounts { get; set; }
    public List<UserOAuthProviderDto> OAuthProviders { get; set; } = [];

    public bool HasPassword { get; set; }
    public DateTimeOffset? PasswordChangeDate { get; set; }
    
    public PersonalDataDto? PersonalData { get; set; }
}