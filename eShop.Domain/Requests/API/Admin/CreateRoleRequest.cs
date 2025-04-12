namespace eShop.Domain.Requests.API.Admin;

public record CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}