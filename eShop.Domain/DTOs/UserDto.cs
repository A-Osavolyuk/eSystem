namespace eShop.Domain.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public DateTimeOffset? UserNameChangeDate { get; set; }

    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset? EmailChangeDate { get; set; }
    public DateTimeOffset? EmailConfirmationDate { get; set; }

    public string? PhoneNumber { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public DateTimeOffset? PhoneNumberChangeDate { get; set; }
    public DateTimeOffset? PhoneNumberConfirmationDate { get; set; }
}