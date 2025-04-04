namespace eShop.Domain.Requests.Api.Admin;

public record class RemoveUserRolesRequest
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}