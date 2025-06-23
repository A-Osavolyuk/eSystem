using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public sealed record GetTwoFactorStateQuery(Guid Id)
    : IRequest<Result>;

public sealed class GetTwoFactorAuthenticationStateQueryHandler(
    IUserManager userManager,
    ICacheService cacheService)
    : IRequestHandler<GetTwoFactorStateQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result> Handle(
        GetTwoFactorStateQuery request, CancellationToken cancellationToken)
    {
        var key = $"2fa-state-{request.Id}";
        var state = await cacheService.GetAsync<TwoFactorStateDto>(key);

        if (state is null)
        {
            var user = await userManager.FindByIdAsync(request.Id, cancellationToken);

            if (user is null)
            {
                return Results.NotFound($"Cannot find user with ID {request.Id}.");
            }

            state = new TwoFactorStateDto() { Enabled = user.TwoFactorEnabled };
            await cacheService.SetAsync(key, state, TimeSpan.FromHours(6));
        }
        
        return Result.Success(state);
    }
}