using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Files;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Commands;

internal sealed record UploadProductImagesCommand(IFormFileCollection Files, Guid ProductId)
    : IRequest<Result>;

internal sealed class UploadProductImagesCommandHandler(
    IStoreService service) : IRequestHandler<UploadProductImagesCommand, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(UploadProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        var list = await service.UploadProductImagesAsync(request.Files, request.ProductId);

        if (!list.Any())
        {
            return Results.InternalServerError($"Cannot upload images for product with ID {request.ProductId}.");
        }

        return Result.Success(new UploadProductImagesResponse()
        {
            Images = list,
            Message = "Files were uploaded successfully.",
        });
    }
}