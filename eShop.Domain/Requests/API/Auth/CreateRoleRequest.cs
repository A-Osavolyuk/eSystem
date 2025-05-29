namespace eShop.Domain.Requests.API.Auth;

public record CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}