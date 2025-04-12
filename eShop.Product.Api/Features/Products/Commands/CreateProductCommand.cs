using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Product;

namespace eShop.Product.Api.Features.Products.Commands;

internal sealed record CreateProductCommand(CreateProductRequest Request) : IRequest<Result>;

internal sealed class CreateProductCommandHandler(
    AppDbContext context) : IRequestHandler<CreateProductCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        if (!await context.Brands.AsNoTracking().AnyAsync(x => x.Id == request.Request.Brand.Id, cancellationToken))
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find brand with ID {request.Request.Brand.Id}"
            });
        }

        if (!await context.Sellers.AsNoTracking().AnyAsync(x => x.Id == request.Request.Seller.Id, cancellationToken))
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find seller with ID {request.Request.Seller.Id}"
            });
        }

        var entity = request.Request.ProductType switch
        {
            ProductTypes.Clothing => Mapper.ToClothingEntity(request.Request),
            ProductTypes.Shoes => Mapper.ToShoesEntity(request.Request),
            _ or ProductTypes.None => Mapper.ToProductEntity(request.Request)
        };

        await context.Products.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Product created successfully");
    }
}