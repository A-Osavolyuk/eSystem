namespace eShop.Auth.Api.Features.TwoFactor.Queries;

public record GetProvidersQuery(string Email) : IRequest<Result>;

public class GetProvidersQueryHandler(
    ITwoFactorManager twoFactorManager,
    IUserManager userManager) : IRequestHandler<GetProvidersQuery, Result>
{
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var providers = await twoFactorManager.GetProvidersAsync(user, cancellationToken);
        var result = providers.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}