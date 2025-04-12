namespace eShop.Domain.Responses.API.Admin;

public record class UserRolesResponse
{
    public Guid UserId { get; set; }
    public IList<RoleData> Roles { get; set; } = new List<RoleData>();
}