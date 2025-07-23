namespace eShop.Infrastructure.Routing;

public class Router
{
    public List<PageRoute> Pages { get; set; } = [];

    public string OnNotFound { get; set; } = "/error?code=404";
    public string OnForbidden { get; set; } = "/error?code=403";
    public string OnNotAuthorized { get; set; } = "/error?code=401";
}