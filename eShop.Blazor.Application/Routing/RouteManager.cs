using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using eShop.Blazor.Application.State;
using Microsoft.AspNetCore.Components;

namespace eShop.Blazor.Application.Routing;

public partial class RouteManager(
    NavigationManager navigationManager,
    RouteOptions routeOptions,
    UserState userState)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly UserState userState = userState;
    private readonly RouteOptions routeOptions = routeOptions;

    public string Uri => navigationManager.Uri;
    public string BaseUri => navigationManager.BaseUri;
    
    public void ExternalRoute(string url) => navigationManager.NavigateTo(url);

    public void Route(string url, bool isRedirect = false, bool forceLoad = false)
    {
        var page = MatchRoute(url, routeOptions.Pages);
        var route = GetRoute(page, url, isRedirect);
        navigationManager.NavigateTo(route, forceLoad);
    }

    private string GetRoute(PageRoute? page, string url, bool isRedirect = false)
    {
        if (page is null)
        {
            return OnError(ErrorCode.NotFound);
        }

        if (page.RequireAuthorization && !userState.IsAuthenticated)
        {
            return OnError(ErrorCode.Unauthorized, url);
        }
        
        if (page is { RequiredRoles.Count: > 0 } or { RequiredPermissions.Count: > 0 })
        {
            if (!userState.Identity!.HasAnyRole(page.RequiredRoles.ToArray()) 
                || !userState.Identity.HasAnyPermission(page.RequiredPermissions.ToArray()))
            {
                var currentPage = new StringBuilder(Uri).Replace(BaseUri, "").ToString();
                return OnError(ErrorCode.Forbidden, currentPage); 
            }
        }

        return isRedirect ? HttpUtility.UrlDecode(url) : url;
    }
    
    private string OnError(ErrorCode errorCode, string? returnUrl = null)
    {
        var baseUrl = errorCode switch
        {
            ErrorCode.NotFound => routeOptions.OnNotFound,
            ErrorCode.Forbidden => routeOptions.OnForbidden,
            ErrorCode.Unauthorized => routeOptions.OnUnauthorized,
            _ => throw new NotSupportedException("Unsupported error code"),
        };
        
        var url = new StringBuilder(baseUrl);

        if (!string.IsNullOrEmpty(returnUrl))
        {
            var encodedUrl = HttpUtility.UrlEncode(returnUrl);
            url.Append("&returnUrl=").Append(encodedUrl);
        }
        
        return url.ToString();
    }

    private Regex BuildExpression(string pattern)
    {
        var regex = RoutePattern().Replace(pattern, match =>
        {
            var inner = match.Value.Trim('{', '}');
            
            if (!inner.Contains(':')) return "(?<string>[^/]+)";
            
            var parts = inner.Split(':');
            
            return parts[1] switch
            {
                "int" => @"(?<int>\d+)",
                "guid" => @"(?<guid>[0-9a-fA-F\-]{36})",
                "*" => "(?<slug>.+)",
                _ => "(?<string>[^/]+)"
            };

        });

        return new Regex("^" + regex + "$", RegexOptions.Compiled);
    }

    private PageRoute? MatchRoute(string route, List<PageRoute> patters)
    {
        var path = route.Split('?').First();

        foreach (var pattern in patters)
        {
            foreach (var uri in pattern.Routes)
            {
                var regex = BuildExpression(uri);

                if (regex.IsMatch(path))
                    return pattern;
            }
        }

        return null;
    }

    [GeneratedRegex(@"\{[^}]+\}")]
    private static partial Regex RoutePattern();
}