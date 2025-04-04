namespace eShop.Product.Api.Features.Brands.Commands;

internal sealed record DeleteBrandCommand(DeleteBrandRequest Request) : IRequest<Result>;

internal sealed class DeleteBrandCommandHandler(
    AppDbContext context) : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(DeleteBrandCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await context.Brands.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Request.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find brand with ID {request.Request.Id}"
            });
        }

        context.Brands.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Brand was successfully deleted");
    }
}