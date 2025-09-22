using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    IStorage storage,
    IHttpContextAccessor httpContextAccessor,
    IUserService userService,
    TokenProvider tokenProvider,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly IStorage storage = storage;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly UserState userState = userState;
    private readonly IUserService userService = userService;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var principal = httpContextAccessor.HttpContext!.User;
            var authenticationState = new AuthenticationState(principal);
            return Task.FromResult(authenticationState);
        }
        
        return Task.FromResult(anonymous);
    }

    public async Task SignInAsync(ClaimsPrincipal principal, HttpContext httpContext)
    {
        var properties = new AuthenticationProperties() { IsPersistent = true, };
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        await LoadAsync(principal.Claims.ToList());
        
        var state = new AuthenticationState(principal);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public async Task NotifyAsync()
    {
        var state = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public async Task SignOutAsync()
    {
        await storage.ClearAsync();

        userState.Clear();
        tokenProvider.Clear();

        await UnauthorizeAsync();
    }
    
    private async Task LoadAsync(List<Claim> claims)
    {
        var userId = Guid.Parse(claims.Single(x => x.Type == AppClaimTypes.Subject).Value);
        var result = await userService.GetUserStateAsync(userId);

        if (result.Success)
        {
            var state = result.Get<UserStateDto>()!;
            userState.Map(state);
        }
    }

    private async Task<AuthenticationState> UnauthorizeAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));

        return await Task.FromResult(anonymous);
    }
}