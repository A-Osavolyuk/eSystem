using eSecurity.Core.Security.Identity;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class PersonalDataEntity : Entity
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleName { get; set; }
    public required Gender Gender { get; set; }
    public required DateTime BirthDate { get; set; }
    
    public UserEntity User { get; set; } = null!;

    public string Fullname => string.IsNullOrEmpty(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";
}