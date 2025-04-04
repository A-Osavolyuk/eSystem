using eShop.Domain.Interfaces.API;

namespace eShop.Auth.Api.Features.Security.Queries;

internal sealed record GetExternalProvidersQuery() : IRequest<Result<IEnumerable<ExternalProviderDto>>>;

internal sealed class GetExternalProvidersQueryHandler(
    AppManager appManager,
    ICacheService cacheService)
    : IRequestHandler<GetExternalProvidersQuery, Result<IEnumerable<ExternalProviderDto>>>
{
    private readonly AppManager appManager = appManager;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result<IEnumerable<ExternalProviderDto>>> Handle(GetExternalProvidersQuery request,
        CancellationToken cancellationToken)
    {
        var key = "external-providers";
        var result = await cacheService.GetAsync<IEnumerable<ExternalProviderDto>>(key);

        if (result is null)
        {
            var schemes = await appManager.SignInManager.GetExternalAuthenticationSchemesAsync();
            var providers = schemes.Select(p => new ExternalProviderDto() { Name = p.Name }).ToList();

            await cacheService.SetAsync(key, providers, TimeSpan.FromHours(6));

            return new Result<IEnumerable<ExternalProviderDto>>(providers);
        }

        return new Result<IEnumerable<ExternalProviderDto>>(result);
    }
}