using eSystem.Core.Enums;

namespace eSystem.Core.DTOs;

public class UserPersonalDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; } = new(1980, 1, 1);
    public DateTimeOffset? UpdateDate { get; set; }
}