namespace eShop.Auth.Api.Entities;

public class PersonalDataEntity : Entity
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Unspecified;
    public DateTime BirthDate { get; set; } = new(1980, 1, 1);
    public UserEntity User { get; set; } = null!;
}