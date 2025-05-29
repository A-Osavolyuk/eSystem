namespace eShop.Domain.Requests.API.Auth;

public record AssignRoleRequest
{
    public Guid UserId { get; set; } 
    public string RoleName { get; set; } = string.Empty;
}