namespace eShop.Domain.Models;

public class PersonalDataModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}