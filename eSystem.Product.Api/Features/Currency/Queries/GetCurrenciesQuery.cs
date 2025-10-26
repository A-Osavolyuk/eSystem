using eSystem.Core.Common.Results;
using eSystem.Product.Api.Interfaces;
using eSystem.Product.Api.Mapping;

namespace eSystem.Product.Api.Features.Currency.Queries;

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