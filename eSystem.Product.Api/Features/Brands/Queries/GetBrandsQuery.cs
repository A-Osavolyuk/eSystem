using eSystem.Core.Common.Results;

namespace eSystem.Product.Api.Features.Brands.Queries;

public record GetBrandsQuery() : IRequest<Result>;

public class GetBrandsQueryHandler(IBrandManager brandManager) : IRequestHandler<GetBrandsQuery, Result>
{
    private readonly IBrandManager brandManager = brandManager;

    public async Task<Result> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        var entities = await brandManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}