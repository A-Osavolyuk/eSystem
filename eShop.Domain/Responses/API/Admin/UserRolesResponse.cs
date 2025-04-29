namespace eShop.Domain.Responses.API.Admin;

public record UserRolesResponse
{
    public Guid UserId { get; set; }
    public IList<RoleData> Roles { get; set; } = new List<RoleData>();
}