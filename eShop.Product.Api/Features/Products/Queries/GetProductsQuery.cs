using eShop.Domain.DTOs;
using eShop.Domain.Interfaces.API;

namespace eShop.Product.Api.Features.Products.Queries;

internal sealed record GetProductsQuery() : IRequest<Result>;

internal sealed class GetProductsQueryHandler(AppDbContext context, ICacheService cacheService)
    : IRequestHandler<GetProductsQuery, Result>
{
    private readonly AppDbContext context = context;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var key = "products";
        var cache = await cacheService.GetAsync<List<ProductDto>>(key);

        if (!cache!.Any())
        {
            var products = await context.Products
                .AsNoTracking()
                .Include(p => p.Seller)
                .Include(p => p.Brand)
                .ToListAsync(cancellationToken);

            var response = products
                .Select(Mapper.ToProductDto)
                .ToList();

            await cacheService.SetAsync(key, response, TimeSpan.FromMinutes(30));

            return Result.Success(response);
        }

        return Result.Success(cache!);
    }
}