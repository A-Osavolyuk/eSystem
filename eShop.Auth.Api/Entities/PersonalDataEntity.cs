using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Entities;

public record PersonalDataEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Gender Gender { get; init; } = Gender.Unspecified;
    public DateTime DateOfBirth { get; init; } = new(1980, 1, 1);
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}