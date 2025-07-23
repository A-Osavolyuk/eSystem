using System.Text.RegularExpressions;
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

    public async Task NavigateAsync(string url, string? returnUri = null)
    {
        var returnPath = new StringBuilder(Uri).Replace(BaseUri, "").ToString();
        
        var route = MatchPattern(url, router.Pages);

        if (route is null)
        {
            NotFound();
            return;
        }

        var state = await authenticationManager.GetStateAsync();
        
        if (route!.RequireAuthorization && (state is null || !state.IsAuthenticated))
        {
            NotAuthorized(returnUri ?? returnPath);
            return;
        }

        if (route is { RequiredRoles.Count: > 0 } or { RequiredPermissions.Count: > 0 })
        {
            if (!state!.HasRole(route.RequiredRoles) || !state.HasPermission(route.RequiredPermissions))
            {
                Forbidden(returnUri ?? returnPath);
                return;
            }
        }

        navigationManager.NavigateTo(url);
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

    private PageRoute? MatchPattern(string route, List<PageRoute> patters)
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

    private void NotFound() => navigationManager.NavigateTo(router.OnNotFound);

    private void Forbidden(string? returnUri)
    {
        var uri = string.IsNullOrEmpty(returnUri) 
            ? router.OnForbidden
            : $"{router.OnForbidden}&returnUri=/{returnUri}";
        
        navigationManager.NavigateTo(uri);
    }

    private void NotAuthorized(string? returnUri)
    {
        var uri = string.IsNullOrEmpty(returnUri) 
            ? router.OnNotAuthorized
            : $"{router.OnNotAuthorized}&returnUri=/{returnUri}";
        
        navigationManager.NavigateTo(uri);
    }
}