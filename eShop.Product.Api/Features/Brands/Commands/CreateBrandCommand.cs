namespace eShop.Product.Api.Features.Brands.Commands;

internal sealed record CreateBrandCommand(CreateBrandRequest Request) : IRequest<Result<CreateBrandResponse>>;

internal sealed class CreateBrandCommandHandle(
    AppDbContext context) : IRequestHandler<CreateBrandCommand, Result<CreateBrandResponse>>
{
    private readonly AppDbContext context = context;

    public async Task<Result<CreateBrandResponse>> Handle(CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var entity = Mapper.ToBrandEntity(request.Request);
        await context.Brands.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new Result<CreateBrandResponse>(new CreateBrandResponse()
        {
            Message = "Successfully created brand"
        });
    }
}