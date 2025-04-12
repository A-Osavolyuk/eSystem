using eShop.Domain.Common.Api;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Commands;

internal sealed record DeleteProductImagesCommand(Guid ProductId) : IRequest<Result>;

internal sealed class DeleteProductImagesCommandHandler(IStoreService service)
    : IRequestHandler<DeleteProductImagesCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(DeleteProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        await service.DeleteProductImagesAsync(request.ProductId);

        return Result.Success("Product images were deleted successfully.");
    }
}