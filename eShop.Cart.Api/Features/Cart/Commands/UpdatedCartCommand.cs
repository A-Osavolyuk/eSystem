using eShop.Cart.Api.Entities;
using eShop.Domain.Enums;

namespace eShop.Cart.Api.Features.Cart.Commands;

internal sealed record UpdatedCartCommand(UpdateCartRequest Request) : IRequest<Result>;

internal sealed class UpdatedCartCommandHandler(DbClient client)
    : IRequestHandler<UpdatedCartCommand, Result>
{
    private readonly DbClient client = client;

    public async Task<Result> Handle(UpdatedCartCommand request,
        CancellationToken cancellationToken)
    {
        var cartCollection = client.GetCollection<CartEntity>("Carts");

        var cart = await cartCollection.Find(x => x.Id == request.Request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find cart with ID {request.Request.Id}."
            });
        }
        else
        {
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
}