using eShop.Domain.DTOs;

namespace eShop.Product.Api.Features.Products.Queries;

internal sealed record GetProductsQuery() : IRequest<Result<List<ProductDto>>>;

internal sealed class GetProductsQueryHandler(AppDbContext context, ICacheService cacheService)
    : IRequestHandler<GetProductsQuery, Result<List<ProductDto>>>
{
    private readonly AppDbContext context = context;
    private readonly ICacheService cacheService = cacheService;

    public async Task<Result<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
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

            return new Result<List<ProductDto>>(response);
        }

        return new Result<List<ProductDto>>(cache!);
    }
}