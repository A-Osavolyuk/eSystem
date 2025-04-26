namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record Get2FaStateQuery(string Email)
    : IRequest<Result>;

internal sealed class GetTwoFactorAuthenticationStateQueryHandler(
    UserManager<UserEntity> userManager,
    ICacheService cacheService)
    : IRequestHandler<Get2FaStateQuery, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result> Handle(
        Get2FaStateQuery request, CancellationToken cancellationToken)
    {
        var key = $"2fa-stata-{request.Email}";
        var state = await cacheService.GetAsync<TwoFactorAuthenticationState>(key);

        if (state is null)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Results.NotFound($"Cannot find user with email {request.Email}.");
            }

            state = new TwoFactorAuthenticationState() { Enabled = user.TwoFactorEnabled };
            await cacheService.SetAsync(key, state, TimeSpan.FromHours(6));
        }
        
        return Result.Success(state.Enabled ? "Two factor authentication state is enabled."
            : "Two factor authentication state is disabled.");
    }
}