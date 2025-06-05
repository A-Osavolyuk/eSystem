using eShop.Domain.Common.API;
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Commands;

internal sealed record DeleteProductImagesCommand(Guid ProductId) : IRequest<Result>;

internal sealed class DeleteProductImagesCommandHandler(IStoreService service)
    : IRequestHandler<DeleteProductImagesCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(DeleteProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(request.ProductId.ToString(), Container.Product);

        return Result.Success("Product images were deleted successfully.");
    }
}