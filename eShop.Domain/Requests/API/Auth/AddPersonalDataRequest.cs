namespace eShop.Domain.Requests.API.Auth;

public class AddPersonalDataRequest
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
}