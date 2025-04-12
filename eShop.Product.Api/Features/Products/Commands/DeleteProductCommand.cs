using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Product;

namespace eShop.Product.Api.Features.Products.Commands;

internal sealed record DeleteProductCommand(DeleteProductRequest Request) : IRequest<Result>;

internal sealed class DeleteProductCommandHandler(AppDbContext context)
    : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly AppDbContext context = context;

    public async Task<Result> Handle(DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await context.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Request.ProductId, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find product with ID {request.Request.ProductId}"
            });
        }

        context.Products.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success("Product was successfully deleted");
    }
}