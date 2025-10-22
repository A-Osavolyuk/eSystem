using eShop.Domain.Common.Results;

namespace eShop.Product.Api.Features.Currency.Queries;

public record GetCurrenciesQuery() : IRequest<Result>;

public class GetCurrenciesQueryHandler(ICurrencyManager currencyManager) : IRequestHandler<GetCurrenciesQuery, Result>
{
    private readonly ICurrencyManager currencyManager = currencyManager;

    public async Task<Result> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var entities = await currencyManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}