namespace eShop.Domain.Requests.Api.Admin;

public record CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}