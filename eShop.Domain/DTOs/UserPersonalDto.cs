namespace eShop.Domain.DTOs;

public class UserPersonalDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; } = new(1980, 1, 1);
    public DateTimeOffset? UpdateDate { get; set; }
}