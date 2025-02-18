using eShop.Domain.Exceptions;
using eShop.Domain.Responses.Api.Files;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Commands;

internal sealed record UploadProductImagesCommand(IFormFileCollection Files, Guid ProductId)
    : IRequest<Result<UploadProductImagesResponse>>;

internal sealed class UploadProductImagesCommandHandler(
    IStoreService service) : IRequestHandler<UploadProductImagesCommand, Result<UploadProductImagesResponse>>
{
    private readonly IStoreService service = service;

    public async Task<Result<UploadProductImagesResponse>> Handle(UploadProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        var list = await service.UploadProductImagesAsync(request.Files, request.ProductId);

        if (!list.Any())
        {
            return new(new FailedOperationException($"Cannot upload images for product with ID {request.ProductId}."));
        }

        return new(new UploadProductImagesResponse()
        {
            Images = list,
            Message = "Files were uploaded successfully.",
        });
    }
}