namespace eShop.Domain.DTOs;

public class UserPasswordDto
{
    public bool HasPassword { get; set; }
    public DateTimeOffset? PasswordChangeDate { get; set; }
}