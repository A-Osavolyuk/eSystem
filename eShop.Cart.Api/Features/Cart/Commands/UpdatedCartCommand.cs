using eShop.Cart.Api.Entities;
using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Cart;

namespace eShop.Cart.Api.Features.Cart.Commands;

internal sealed record UpdatedCartCommand(UpdateCartRequest Request) : IRequest<Result>;

internal sealed class UpdatedCartCommandHandler(IMongoDatabase client)
    : IRequestHandler<UpdatedCartCommand, Result>
{
    private readonly IMongoDatabase client = client;

    public async Task<Result> Handle(UpdatedCartCommand request,
        CancellationToken cancellationToken)
    {
        var cartCollection = client.GetCollection<CartEntity>("Carts");

        var cart = await cartCollection.Find(x => x.Id == request.Request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return Results.NotFound($"Cannot find cart with ID {request.Request.Id}.");
        }

        var newCart = new CartEntity
        {
            Id = cart.Id,
            UserId = cart.UserId,
            ItemsCount = request.Request.ItemsCount,
            UpdateDate = DateTime.Now,
            CreateDate = cart.CreateDate,
            Items = request.Request.Items
        };

        await cartCollection.ReplaceOneAsync(x => x.Id == request.Request.Id, newCart,
            cancellationToken: cancellationToken);

        return Result.Success("Cart was successfully updated");
    }
}