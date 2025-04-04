namespace eShop.Domain.Requests.Api.Auth;

public record class ChangePersonalDataRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; } = new DateTime(1980, 1, 1);
}