namespace eShop.Domain.Requests.API.Admin;

public record DeleteRoleRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}