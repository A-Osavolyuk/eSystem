namespace eShop.Domain.Types;

public record PersonalData : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = new(1980, 1, 1);
}