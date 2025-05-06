namespace eShop.Domain.Responses.API.Admin;

public record UserRolesResponse
{
    public Guid UserId { get; set; }
    public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
}