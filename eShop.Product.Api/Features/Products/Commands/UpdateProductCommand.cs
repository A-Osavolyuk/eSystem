using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Product;
using Results = eShop.Domain.Common.API.Results;

namespace eShop.Product.Api.Features.Products.Commands;

internal sealed record UpdateProductCommand(UpdateProductRequest Request) : IRequest<Result>;

internal sealed class UpdateProductCommandHandler(AppDbContext context)
    : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        if (!await context.Products.AsNoTracking().AnyAsync(x => x.Id == request.Request.Id, cancellationToken))
        {
            return Results.NotFound($"Cannot find product with ID {request.Request.Id}");
        }

        if (!await context.Brands.AsNoTracking().AnyAsync(x => x.Id == request.Request.Brand.Id, cancellationToken))
        {
            return Results.NotFound($"Cannot find brand with ID {request.Request.Brand.Id}");
        }

        if (!await context.Sellers.AsNoTracking().AnyAsync(x => x.Id == request.Request.Seller.Id, cancellationToken))
        {
            return Results.NotFound($"Cannot find seller with ID {request.Request.Seller.Id}");
        }

        var entity = request.Request.ProductType switch
        {
            ProductTypes.Clothing => Mapper.ToClothingEntity(request.Request),
            ProductTypes.Shoes => Mapper.ToShoesEntity(request.Request),
            _ or ProductTypes.None => Mapper.ToProductEntity(request.Request)
        };

        context.Products.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success("Product was updated successfully.");
    }
}