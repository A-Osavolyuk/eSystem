namespace eShop.Auth.Api.Features.Auth.Queries;

internal sealed record Get2FaStateQuery(string Email)
    : IRequest<Result<TwoFactorAuthenticationState>>;

internal sealed class GetTwoFactorAuthenticationStateQueryHandler(
    AppManager appManager,
    ICacheService cacheService)
    : IRequestHandler<Get2FaStateQuery, Result<TwoFactorAuthenticationState>>
{
    private readonly AppManager appManager = appManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result<TwoFactorAuthenticationState>> Handle(
        Get2FaStateQuery request, CancellationToken cancellationToken)
    {
        var key = $"2fa-stata-{request.Email}";
        var state = await cacheService.GetAsync<TwoFactorAuthenticationState>(key);

        if (state is null)
        {
            var user = await appManager.UserManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new(new NotFoundException($"Cannot find user with email {request.Email}."));
            }

            state = new TwoFactorAuthenticationState() { Enabled = user.TwoFactorEnabled };
            await cacheService.SetAsync(key, state, TimeSpan.FromHours(6));

            return new(state);
        }

        return new(state);
    }
}