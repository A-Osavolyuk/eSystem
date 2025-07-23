namespace eShop.Infrastructure.Routing;

public class Router
{
    public List<PageRoute> Pages { get; set; } = [];

    public string NotFoundPageUri { get; set; } = "/error?code=404";
    public string ForbiddenPageUri { get; set; } = "/error?code=403";
    public string NotAuthorizedPageUri { get; set; } = "/error?code=401";
}