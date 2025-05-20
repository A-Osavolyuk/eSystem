namespace eShop.Domain.Requests.API.Admin;

public record UnassignRolesRequest
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = [];
}