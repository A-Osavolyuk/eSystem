using System.Text.RegularExpressions;
using System.Web;
using eShop.Domain.Common.Security;
using eShop.Infrastructure.Security;
using Microsoft.AspNetCore.Components;

namespace eShop.Infrastructure.Routing;

public class RouteManager(
    NavigationManager navigationManager,
    Router router,
    AuthenticationStateManager authenticationStateManager)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationStateManager authenticationStateManager = authenticationStateManager;
    private readonly Router router = router;

    public string Uri => navigationManager.Uri;
    public string BaseUri => navigationManager.BaseUri;

    public async Task NavigateAsync(string url, bool isRedirect = false, bool forceLoad = false, bool isExternal = false)
    {
        if (isExternal)
        {
            navigationManager.NavigateTo(url, forceLoad);
            return;
        }
        
        var route = MatchRoute(url, router.Pages);

        if (route is null)
        {
            OnError(ErrorCode.NotFound, forceLoad: forceLoad); 
            return;
        }

        var state = await authenticationStateManager.GetStateAsync();

        if (route!.RequireAuthorization && (state is null || !state.IsAuthenticated))
        {
            OnError(ErrorCode.Unauthorized, url, forceLoad); 
            return;
        }

        if (route is { RequiredRoles.Count: > 0 } or { RequiredPermissions.Count: > 0 })
        {
            if (!state!.HasRole(route.RequiredRoles) || !state.HasPermission(route.RequiredPermissions))
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
            ErrorCode.NotFound => router.OnNotFound,
            ErrorCode.Forbidden => router.OnForbidden,
            ErrorCode.Unauthorized => router.OnUnauthorized,
            _ => throw new NotSupportedException("Unsupported error code"),
        };
        
        var url = new StringBuilder(baseUrl);

        if (!string.IsNullOrEmpty(returnUrl))
        {
            var encodedUrl = HttpUtility.UrlEncode(returnUrl);
            url.Append("?returnUrl=").Append(encodedUrl);
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