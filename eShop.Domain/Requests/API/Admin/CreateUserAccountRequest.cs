namespace eShop.Domain.Requests.Api.Admin;

public record class CreateUserAccountRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = new DateTime(1980, 1, 1);

    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}