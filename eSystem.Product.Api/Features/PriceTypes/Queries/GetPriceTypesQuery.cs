using eSystem.Core.Common.Results;
using eSystem.Product.Api.Interfaces;
using eSystem.Product.Api.Mapping;

namespace eSystem.Product.Api.Features.PriceTypes.Queries;

public record GetPriceTypesQuery : IRequest<Result>;

public class GetPriceTypesQueryHandler(IPriceManager priceManager) : IRequestHandler<GetPriceTypesQuery, Result>
{
    private readonly IPriceManager priceManager = priceManager;

    public async Task<Result> Handle(GetPriceTypesQuery request, CancellationToken cancellationToken)
    {
        var entities = await priceManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}