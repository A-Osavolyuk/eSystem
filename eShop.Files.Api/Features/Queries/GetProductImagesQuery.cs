using eShop.Domain.Common.API;
using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Features.Queries;

internal sealed record GetProductImagesQuery(Guid ProductId) : IRequest<Result>;

internal sealed class GetProductImagesQueryHandler(IStoreService service)
    : IRequestHandler<GetProductImagesQuery, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
    {
        var prefix = request.ProductId.ToString();
        var response = await service.FindManyAsync(prefix, Container.Product);

        if (response.Count == 0)
        {
            return Results.NotFound($"Cannot find product images for product with ID {request.ProductId}.");
        }

        return Result.Success(response);
    }
}