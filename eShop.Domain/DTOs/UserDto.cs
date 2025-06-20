namespace eShop.Domain.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool TwoFactorEnabled { get; set; }
    
    public DateTimeOffset? PasswordChangeDate { get; set; }
    public DateTimeOffset? EmailChangeDate { get; set; }
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    public DateTimeOffset? UserNameChangeDate { get; set; }

    public PersonalDataDto? PersonalData { get; set; }
}