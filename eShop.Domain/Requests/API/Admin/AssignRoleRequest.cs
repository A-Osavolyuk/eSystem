namespace eShop.Domain.Requests.Api.Admin;

public record class AssignRoleRequest
{
    public Guid UserId { get; set; } 
    public string RoleName { get; set; } = string.Empty;
}