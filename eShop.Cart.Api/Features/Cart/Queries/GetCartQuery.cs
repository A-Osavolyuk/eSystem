using eShop.Cart.Api.Entities;
using eShop.Cart.Api.Mapping;
using eShop.Domain.Common.API;

namespace eShop.Cart.Api.Features.Cart.Queries;

internal sealed record GetCartQuery(Guid UserId) : IRequest<Result>;

internal sealed class GetCartQueryHandler(IMongoDatabase client) : IRequestHandler<GetCartQuery, Result>
{
    private readonly IMongoDatabase client = client;

    public async Task<Result> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<CartEntity>("Carts");
        var cart = await collection.Find(x => x.UserId == request.UserId).FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return Results.NotFound($"Cannot find cart with user ID {request.UserId}.");
        }

        return Result.Success(Mapper.Map(cart));
    }
}