namespace eShop.Auth.Api.Features.Auth.Queries;

internal sealed record GetTwoFactorAuthenticationStateQuery(string Email)
    : IRequest<Result<TwoFactorAuthenticationStateResponse>>;

internal sealed class GetTwoFactorAuthenticationStateQueryHandler(
    AppManager appManager,
    ICacheService cacheService)
    : IRequestHandler<GetTwoFactorAuthenticationStateQuery, Result<TwoFactorAuthenticationStateResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result<TwoFactorAuthenticationStateResponse>> Handle(
        GetTwoFactorAuthenticationStateQuery request, CancellationToken cancellationToken)
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

            return new(new TwoFactorAuthenticationStateResponse()
            {
                State = state
            });
        }

        return new Result<TwoFactorAuthenticationStateResponse>(new TwoFactorAuthenticationStateResponse()
        {
            State = state
        });
    }
}