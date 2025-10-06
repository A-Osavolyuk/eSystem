namespace eShop.Domain.Requests.Auth;

public class UpdateRoleRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}