using System.Security.Claims;
using eSecurity.Common.DTOs;
using eSecurity.Common.JS.Fetch;
using eSecurity.Common.Routing;
using eSecurity.Common.State.States;
using eSecurity.Common.Storage;
using eSecurity.Features.Users.Queries;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace eSecurity.Security.Authentication;

public sealed class AuthenticationManager(
    NavigationManager navigationManager,
    AuthenticationStateProvider authenticationStateProvider,
    UserState userState,
    ISender sender,
    IFetchClient fetchClient,
    IStorage storage)
{
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
    private readonly UserState userState = userState;
    private readonly ISender sender = sender;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly IStorage storage = storage;

    public async Task SignInAsync()
    {
        var claimsResult = await sender.Send(new GetUserClaimsQuery(userState.UserId));

        if (claimsResult.Succeeded)
        {
            var claims = claimsResult.Get<List<Claim>>();
            var identity = new SignIdentity()
            {
                Scheme = CookieAuthenticationDefaults.AuthenticationScheme,
                Claims = claims.Select(x => new ClaimValue() { Type = x.Type, Value = x.Value }).ToList()
            };
            
            var fetchOptions = new FetchOptions()
            {
                Url = $"{navigationManager.BaseUri}api/authentication/sign-in",
                Method = HttpMethod.Post,
                Body = identity
            };

            var result = await fetchClient.FetchAsync(fetchOptions);
            if (result.Succeeded)
            {
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                (authenticationStateProvider as ClaimAuthenticationStateProvider)!.SignIn(claimsPrincipal);
            }
        }
    }

    public async Task SignOutAsync()
    {
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/authentication/sign-out",
        };

        var result = await fetchClient.FetchAsync(fetchOptions);
        if (result.Succeeded)
        {
            await (authenticationStateProvider as ClaimAuthenticationStateProvider)!.SignOutAsync();
            await storage.ClearAsync();

            navigationManager.NavigateTo(Links.Account.SignIn);
        }
    }
}