using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class PersonalDataEntity : Entity
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Unspecified;
    public DateTime BirthDate { get; set; } = new(1980, 1, 1);
    public UserEntity User { get; set; } = null!;
}