using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.DTOs;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    IStorage storage,
    IHttpContextAccessor httpContextAccessor,
    TokenProvider tokenProvider,
    TokenHandler tokenHandler,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly IStorage storage = storage;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly UserState userState = userState;

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

    public async Task SignInAsync(string accessToken)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            tokenProvider.AccessToken = accessToken;

            var rawToken = tokenHandler.ReadToken(accessToken)!;
            var claims = rawToken.Claims.ToList();

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authenticationState = new AuthenticationState(claimsPrincipal);

            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
        else await UnauthorizeAsync();
    }

    public async Task SignOutAsync()
    {
        await storage.ClearAsync();

        userState.Clear();
        tokenProvider.Clear();

        await UnauthorizeAsync();
    }

    private async Task<AuthenticationState> UnauthorizeAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));

        return await Task.FromResult(anonymous);
    }
}