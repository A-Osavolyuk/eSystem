namespace eShop.Auth.Api.Features.Providers.Queries;

public record GetProvidersQuery() : IRequest<Result>;

public class GetProvidersQueryHandler(ITwoFactorProviderManager twoFactorProviderManager) : IRequestHandler<GetProvidersQuery, Result>
{
    private readonly ITwoFactorProviderManager twoFactorProviderManager = twoFactorProviderManager;

    public async Task<Result> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await twoFactorProviderManager.GetAllAsync(cancellationToken);
        var result = providers.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}