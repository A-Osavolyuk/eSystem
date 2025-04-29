namespace eShop.Domain.Requests.API.Admin;

public record CreateUserAccountRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = new(1980, 1, 1);

    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}