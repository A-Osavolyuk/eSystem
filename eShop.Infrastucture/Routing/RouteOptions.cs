namespace eShop.Infrastructure.Routing;

public class RouteOptions
{
    public List<PageRoute> Pages { get; set; } = [];

    public string OnNotFound { get; set; } = "/error?code=404";
    public string OnForbidden { get; set; } = "/error?code=403";
    public string OnUnauthorized { get; set; } = "/error?code=401";
}