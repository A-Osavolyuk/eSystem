using eAccount.Application.State;
using eSystem.Core.DTOs;
using eSystem.Core.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace eAccount.Infrastructure.Security;

public class JwtAuthenticationStateProvider(
    IHttpContextAccessor httpContextAccessor,
    //IUserService userService,
    UserState userState) : AuthenticationStateProvider
{
    private readonly AuthenticationState anonymous = new(new ClaimsPrincipal());
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private readonly UserState userState = userState;
    //private readonly IUserService userService = userService;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (httpContext.User.Identity?.IsAuthenticated != true) return Task.FromResult(anonymous);
        
        var principal = httpContext.User;
        var authenticationState = new AuthenticationState(principal);

        var subjectClaim = principal.Claims.Single(x => x.Type == AppClaimTypes.Subject);
        var userId = Guid.Parse(subjectClaim.Value);
        userState.UserId = userId;
            
        return Task.FromResult(authenticationState);

    }

    // public async Task SignInAsync(ClaimsPrincipal principal)
    // {
    //     var state  = new AuthenticationState(principal);
    //     await LoadStateAsync();
    //     NotifyAuthenticationStateChanged(Task.FromResult(state));
    // }
    public Task SignOutAsync()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        return Task.CompletedTask;
    }
    // private async Task LoadStateAsync()
    // {
    //     var result = await userService.GetUserStateAsync(userState.UserId);
    //     
    //     if (result.Success)
    //     {
    //         var state = result.Get<UserStateDto>()!;
    //         userState.Map(state);
    //     }
    // }
}