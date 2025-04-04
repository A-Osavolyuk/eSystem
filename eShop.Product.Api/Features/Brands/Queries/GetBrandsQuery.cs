using eShop.Domain.DTOs;

namespace eShop.Product.Api.Features.Brands.Queries;

internal sealed record GetBrandsQuery() : IRequest<Result>;

internal sealed class GetBrandsQueryHandler(AppDbContext context)
    : IRequestHandler<GetBrandsQuery, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await context.Brands.AsNoTracking().ToListAsync(cancellationToken);
        var response = brands.Select(Mapper.Map).ToList();
        return Result.Success(response);
    }
}