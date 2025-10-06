namespace eShop.Domain.Requests.Auth;

public record CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}