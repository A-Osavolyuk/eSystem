using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Storage;
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Features.Queries;

internal sealed record GetProductImagesQuery(Guid ProductId) : IRequest<Result>;

internal sealed class GetProductImagesQueryHandler(IStoreService service)
    : IRequestHandler<GetProductImagesQuery, Result>
{
    private readonly IStoreService service = service;

    public async Task<Result> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
    {
        var prefix = request.ProductId.ToString();
        var files = await service.FindManyAsync(prefix, Container.Product);

        if (files.Count == 0)
        {
            return Results.NotFound($"Cannot find product images for product with ID {request.ProductId}.");
        }

        var response = new LoadFiledResponse() { Files = files };

        return Result.Success(response);
    }
}