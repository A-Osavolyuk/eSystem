namespace eShop.Product.Api.Features.Products.Commands;

internal sealed record CreateProductCommand(CreateProductRequest Request) : IRequest<Result<CreateProductResponse>>;

internal sealed class CreateProductCommandHandler(
    AppDbContext context) : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly AppDbContext context = context;

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        if (!await context.Brands.AsNoTracking().AnyAsync(x => x.Id == request.Request.Brand.Id, cancellationToken))
        {
            return new Result<CreateProductResponse>(
                new NotFoundException($"Cannot find brand with ID {request.Request.Brand.Id}"));
        }

        if (!await context.Sellers.AsNoTracking().AnyAsync(x => x.Id == request.Request.Seller.Id, cancellationToken))
        {
            return new Result<CreateProductResponse>(
                new NotFoundException($"Cannot find seller with ID {request.Request.Seller.Id}"));
        }

        var entity = request.Request.ProductType switch
        {
            ProductTypes.Clothing => Mapper.ToClothingEntity(request.Request),
            ProductTypes.Shoes => Mapper.ToShoesEntity(request.Request),
            _ or ProductTypes.None => Mapper.ToProductEntity(request.Request)
        };

        await context.Products.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new Result<CreateProductResponse>(new CreateProductResponse()
        {
            Message = "Product created successfully"
        });
    }
}