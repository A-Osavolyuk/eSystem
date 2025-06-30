namespace eShop.Product.Api.Features.ProductType.Queries;

public record GetProductTypesQuery() : IRequest<Result>;

public class GetProductTypesQueryHandler(ITypeManager typeManager) : IRequestHandler<GetProductTypesQuery, Result>
{
    private readonly ITypeManager typeManager = typeManager;

    public async Task<Result> Handle(GetProductTypesQuery request, CancellationToken cancellationToken)
    {
        var entities = await typeManager.GetAllAsync(cancellationToken);
        return Result.Success(entities);
    }
}