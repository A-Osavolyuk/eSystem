using eShop.Domain.Common.Results;

namespace eShop.Product.Api.Features.ProductTypes.Queries;

public record GetProductTypesQuery() : IRequest<Result>;

public class GetProductTypesQueryHandler(ITypeManager typeManager) : IRequestHandler<GetProductTypesQuery, Result>
{
    private readonly ITypeManager typeManager = typeManager;

    public async Task<Result> Handle(GetProductTypesQuery request, CancellationToken cancellationToken)
    {
        var entities = await typeManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        
        return Result.Success(result);
    }
}