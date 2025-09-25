namespace eShop.Auth.Api.Features.TwoFactor.Queries;

public record GetProvidersQuery() : IRequest<Result>;

public class GetProvidersQueryHandler(ITwoFactorManager twoFactorManager) : IRequestHandler<GetProvidersQuery, Result>
{
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await twoFactorManager.GetAllAsync(cancellationToken);
        var result = providers.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}