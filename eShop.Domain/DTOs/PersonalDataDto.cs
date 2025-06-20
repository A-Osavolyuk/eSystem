using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.DTOs;

public record PersonalDataDto
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; } = new(1980, 1, 1);
    public DateTimeOffset? UpdateDate { get; set; }
}