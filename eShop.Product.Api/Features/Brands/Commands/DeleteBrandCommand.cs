namespace eShop.Product.Api.Features.Brands.Commands;

internal sealed record DeleteBrandCommand(DeleteBrandRequest Request) : IRequest<Result<DeleteBrandResponse>>;

internal sealed class DeleteBrandCommandHandler(
    AppDbContext context) : IRequestHandler<DeleteBrandCommand, Result<DeleteBrandResponse>>
{
    private readonly AppDbContext context = context;

    public async Task<Result<DeleteBrandResponse>> Handle(DeleteBrandCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await context.Brands.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Request.Id, cancellationToken);

        if (entity is null)
        {
            return new Result<DeleteBrandResponse>(
                new NotFoundException($"Cannot find brand with ID {request.Request.Id}"));
        }

        context.Brands.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new Result<DeleteBrandResponse>(new DeleteBrandResponse()
        {
            Message = "Brand was successfully deleted"
        });
    }
}