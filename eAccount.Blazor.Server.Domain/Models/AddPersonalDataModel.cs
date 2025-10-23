namespace eAccount.Blazor.Server.Domain.Models;

public class AddPersonalDataModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}