namespace eShop.Domain.Responses.Api.Auth;

public class PersonalDataResponse
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime DateOfBirth { get; set; } = new DateTime(1980, 1, 1);
}