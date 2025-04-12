namespace eShop.Domain.Requests.API.Admin;

public record class AssignRoleRequest
{
    public Guid UserId { get; set; } 
    public string RoleName { get; set; } = string.Empty;
}