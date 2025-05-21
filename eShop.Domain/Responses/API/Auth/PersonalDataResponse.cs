namespace eShop.Domain.Responses.API.Auth;

public class PersonalDataResponse
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; } = new(1980, 1, 1);
}