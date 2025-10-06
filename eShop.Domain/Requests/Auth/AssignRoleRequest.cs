namespace eShop.Domain.Requests.Auth;

public record AssignRoleRequest
{
    public Guid UserId { get; set; } 
    public string RoleName { get; set; } = string.Empty;
}