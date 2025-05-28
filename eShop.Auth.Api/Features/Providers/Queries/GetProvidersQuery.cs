namespace eShop.Auth.Api.Features.Providers.Queries;

public record GetProvidersQuery() : IRequest<Result>;

public class GetProvidersQueryHandler(IProviderManager providerManager) : IRequestHandler<GetProvidersQuery, Result>
{
    private readonly IProviderManager providerManager = providerManager;

    public async Task<Result> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        var providers = await providerManager.GetProvidersAsync(cancellationToken);
        var result = providers.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}