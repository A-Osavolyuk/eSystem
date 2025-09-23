using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    IUserService userService,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly UserState userState = userState;
    private readonly IUserService userService = userService;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var principal = httpContextAccessor.HttpContext!.User;
            var authenticationState = new AuthenticationState(principal);

            var subjectClaim = principal.Claims.Single(x => x.Type == AppClaimTypes.Subject);
            var userId = Guid.Parse(subjectClaim.Value);
            userState.UserId = userId;
            
            return Task.FromResult(authenticationState);
        }
        
        return Task.FromResult(anonymous);
    }

    public async Task SignInAsync(ClaimsPrincipal principal)
    {
        var state  = new AuthenticationState(principal);
        await LoadStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }
    
    public Task SignOutAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        return Task.CompletedTask;
    }
    
    private async Task LoadStateAsync()
    {
        var result = await userService.GetUserStateAsync(userState.UserId);
        
        if (result.Success)
        {
            var state = result.Get<UserStateDto>()!;
            userState.Map(state);
        }
    }
}