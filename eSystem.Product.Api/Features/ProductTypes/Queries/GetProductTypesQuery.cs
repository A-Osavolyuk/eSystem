using eSystem.Domain.Common.Results;
using eSystem.Product.Api.Interfaces;
using eSystem.Product.Api.Mapping;

namespace eSystem.Product.Api.Features.ProductTypes.Queries;

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