using System.Text.RegularExpressions;
using System.Web;
using eShop.Infrastructure.Security;
using eShop.Infrastructure.State;
using Microsoft.AspNetCore.Components;

namespace eShop.Infrastructure.Routing;

public class RouteManager(
    NavigationManager navigationManager,
    RouteOptions routeOptions,
    UserState userState)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly UserState userState = userState;
    private readonly RouteOptions routeOptions = routeOptions;

    public string Uri => navigationManager.Uri;
    public string BaseUri => navigationManager.BaseUri;

    public void NavigateAsync(string url, bool isRedirect = false, bool forceLoad = false, bool isExternal = false)
    {
        if (isExternal)
        {
            navigationManager.NavigateTo(url, forceLoad);
            return;
        }
        
        var route = MatchRoute(url, routeOptions.Pages);

        if (route is null)
        {
            OnError(ErrorCode.NotFound, forceLoad: forceLoad); 
            return;
        }
        

        if (route!.RequireAuthorization && (!userState.IsAuthenticated))
        {
            OnError(ErrorCode.Unauthorized, url, forceLoad); 
            return;
        }

        if (route is { RequiredRoles.Count: > 0 } or { RequiredPermissions.Count: > 0 })
        {
            if (!userState.HasRole(route.RequiredRoles) || !userState.HasPermission(route.RequiredPermissions))
            {
                var returnUrl = new StringBuilder(Uri).Replace(BaseUri, "").ToString();

                OnError(ErrorCode.Forbidden, returnUrl, forceLoad); 
                return;
            }
        }

        if (isRedirect)
        {
            var encodedUri = HttpUtility.UrlDecode(url);
            navigationManager.NavigateTo(encodedUri, forceLoad);
            return;
        }

        navigationManager.NavigateTo(url, forceLoad);
    }
    
    private void OnError(ErrorCode errorCode, string? returnUrl = null, bool forceLoad = false)
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
        
        navigationManager.NavigateTo(url.ToString(), forceLoad);
    }

    private Regex BuildExpression(string pattern)
    {
        var regex = Regex.Replace(pattern, @"\{[^}]+\}", match =>
        {
            var inner = match.Value.Trim('{', '}');
            if (inner.Contains(":"))
            {
                var parts = inner.Split(':');
                return parts[1] switch
                {
                    "int" => @"(?<int>\d+)",
                    "guid" => @"(?<guid>[0-9a-fA-F\-]{36})",
                    "*" => "(?<slug>.+)",
                    _ => "(?<string>[^/]+)"
                };
            }

            return "(?<string>[^/]+)";
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
}