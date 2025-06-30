using eShop.Product.Api.Mapping;

namespace eShop.Product.Api.Features.Units.Queries;

public record GetUnitsQuery : IRequest<Result>;

public class GetUnitsQueryHandler(IUnitManager unitManager) : IRequestHandler<GetUnitsQuery, Result>
{
    private readonly IUnitManager unitManager = unitManager;

    public async Task<Result> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var entities = await unitManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}