namespace eShop.Blazor.Server.Application.Routing;

public class PageRoute
{
    public List<string> Routes { get; set; } = [];
    public bool RequireAuthorization { get; set; } = false;
    public List<string> RequiredRoles { get; set; } = [];
    public List<string> RequiredPermissions { get; set; } = [];
}