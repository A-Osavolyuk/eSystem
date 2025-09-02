namespace eShop.BlazorWebUI.Models;

public class UserModel
{
    public Guid UserId { get; set; }
    public string? AvatarUri { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public bool HasEmail => !string.IsNullOrEmpty(Email);
    public bool EmailConfirmed  { get; set; }
    public DateTimeOffset? EmailChangeDate { get; set; }
    public DateTimeOffset? EmailConfirmationDate { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    public DateTimeOffset? UserNameChangeDate { get; set; }
    
    public string? PhoneNumber { get; set; } = string.Empty;
    public bool HasPhoneNumber => !string.IsNullOrEmpty(PhoneNumber);
    public bool PhoneNumberConfirmed  { get; set; }
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    public DateTimeOffset? PhoneNumberConfirmationDate { get; set; }
}