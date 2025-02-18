namespace eShop.Auth.Api.Entities;

public record class PersonalDataEntity : IAuditable, IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; } = new DateTime(1980, 1, 1);
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
    [JsonIgnore] public string UserId { get; init; } = string.Empty;
    [JsonIgnore] public AppUser? User { get; init; }
}