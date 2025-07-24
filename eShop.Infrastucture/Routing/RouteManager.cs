using System.Text.RegularExpressions;
using System.Web;
using eShop.Domain.Common.Security;
using eShop.Infrastructure.Security;
using Microsoft.AspNetCore.Components;

namespace eShop.Infrastructure.Routing;

public class RouteManager(
    NavigationManager navigationManager,
    Router router,
    AuthenticationManager authenticationManager)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationManager authenticationManager = authenticationManager;
    private readonly Router router = router;

    public string Uri => navigationManager.Uri;
    public string BaseUri => navigationManager.BaseUri;

    public async Task NavigateAsync(string url, bool isRedirect = false, bool forceLoad = false)
    {
        var route = MatchRoute(url, router.Pages);

        if (route is null)
        {
            NotFound(forceLoad);
            return;
        }

        var state = await authenticationManager.GetStateAsync();

        if (route!.RequireAuthorization && (state is null || !state.IsAuthenticated))
        {
            var returnUrl = HttpUtility.UrlEncode(url);

            NotAuthorized(returnUrl, forceLoad);
            return;
        }

        if (route is { RequiredRoles.Count: > 0 } or { RequiredPermissions.Count: > 0 })
        {
            if (!state!.HasRole(route.RequiredRoles) || !state.HasPermission(route.RequiredPermissions))
            {
                var path = new StringBuilder(Uri).Replace(BaseUri, "").ToString();
                var returnUrl = HttpUtility.UrlEncode(path);

                Forbidden(returnUrl, forceLoad);
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

    private void NotFound(bool forceLoad = false) => navigationManager.NavigateTo(router.OnNotFound, forceLoad);

    private void Forbidden(string? returnUrl, bool forceLoad = false)
    {
        var uri = string.IsNullOrEmpty(returnUrl)
            ? router.OnForbidden
            : $"{router.OnForbidden}&returnUri=/{returnUrl}";

        navigationManager.NavigateTo(uri, forceLoad);
    }

    private void NotAuthorized(string? returnUrl, bool forceLoad = false)
    {
        var uri = string.IsNullOrEmpty(returnUrl)
            ? router.OnNotAuthorized
            : $"{router.OnNotAuthorized}&returnUri=/{returnUrl}";

        navigationManager.NavigateTo(uri, forceLoad);
    }
}