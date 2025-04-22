using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Queries;

internal sealed record GetProductImagesQuery(Guid ProductId) : IRequest<Result>;

internal sealed class GetProductImagesQueryHandler(IStoreService service)
    : IRequestHandler<GetProductImagesQuery, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
    {
        var response = await service.GetProductImagesAsync(request.ProductId);

        if (!response.Any())
        {
            return Results.NotFound($"Cannot find product images for product with ID {request.ProductId}.");
        }

        return Result.Success(response);
    }
}