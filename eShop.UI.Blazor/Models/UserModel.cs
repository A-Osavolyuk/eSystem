namespace eShop.BlazorWebUI.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public bool? TwoFactorEnabled  { get; set; }
    public string? AvatarUri { get; set; } = string.Empty;
    public DateTimeOffset? PasswordChangeDate { get; set; }
    public DateTimeOffset? EmailChangeDate { get; set; }
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    public DateTimeOffset? UserNameChangeDate { get; set; }
    public PersonalDataModel? PersonalData { get; set; }
}