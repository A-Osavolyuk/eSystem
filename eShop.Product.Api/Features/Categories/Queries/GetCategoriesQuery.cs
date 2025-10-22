using eShop.Domain.Common.Results;

namespace eShop.Product.Api.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<Result>;

public class GetCategoriesQueryHandler(ICategoryManager categoryManager) : IRequestHandler<GetCategoriesQuery, Result>
{
    private readonly ICategoryManager categoryManager = categoryManager;

    public async Task<Result> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var entities = await categoryManager.GetAllAsync(cancellationToken);
        var result = entities.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}