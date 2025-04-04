using eShop.Domain.DTOs;
using eShop.Domain.Interfaces.API;
using eShop.Product.Api.Entities;

namespace eShop.Product.Api.Features.Products.Queries;

internal sealed record GetProductByNameQuery(string ProductName) : IRequest<Result>;

internal sealed class GetProductQueryByNameHandler(AppDbContext context, ICacheService cacheService)
    : IRequestHandler<GetProductByNameQuery, Result>
{
    private readonly AppDbContext context = context;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
    {
        var key = $"product-{request.ProductName}";
        var cachedEntity = await cacheService.GetAsync<ProductDto>(key);

        if (cachedEntity is null)
        {
            if (string.IsNullOrEmpty(request.ProductName))
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Not found",
                    Details = $"You must provide a product name in request"
                });
            }

            var entity = await context.Products
                .AsNoTracking()
                .Include(p => p.Seller)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(x => x.Name == request.ProductName, cancellationToken);

            if (entity is null)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Not found",
                    Details = $"Cannot find product {request.ProductName}"
                });
            }

            var response = await Map(entity);

            await cacheService.SetAsync(key, response, TimeSpan.FromMinutes(30));

            return Result.Success(response);
        }

        return Result.Success(cachedEntity!);
    }

    private async Task<ProductDto> Map(ProductEntity entity)
    {
        var response = entity.ProductType switch
        {
            ProductTypes.Shoes => Mapper.ToShoesDto(await FindOfType<ShoesEntity>(entity)),
            ProductTypes.Clothing => Mapper.ToClothingDto(await FindOfType<ClothingEntity>(entity)),
            _ or ProductTypes.None => Mapper.ToProductDto(entity),
        };

        return response;
    }

    private async Task<TEntity> FindOfType<TEntity>(ProductEntity entity) where TEntity : ProductEntity
    {
        var response = await context.Products.AsNoTracking().OfType<TEntity>()
            .FirstOrDefaultAsync(x => x.Article == entity.Article);
        return response!;
    }
}