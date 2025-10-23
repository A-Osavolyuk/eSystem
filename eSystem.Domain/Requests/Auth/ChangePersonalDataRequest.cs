using eSystem.Domain.Enums;

namespace eSystem.Domain.Requests.Auth;

public record ChangePersonalDataRequest
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}