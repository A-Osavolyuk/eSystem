namespace eShop.BlazorWebUI.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset? EmailChangeDate { get; set; }
    
    public string? RecoveryEmail { get; set; } = string.Empty;
    public DateTimeOffset? RecoveryEmailChangeDate { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    public DateTimeOffset? UserNameChangeDate { get; set; }
    
    public string? PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    
    public bool? TwoFactorEnabled  { get; set; }
    public DateTimeOffset? PasswordChangeDate { get; set; }
    public string? AvatarUri { get; set; } = string.Empty;
    
    public PersonalDataModel? PersonalData { get; set; }
}