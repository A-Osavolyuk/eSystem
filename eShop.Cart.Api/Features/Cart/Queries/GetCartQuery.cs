using eShop.Cart.Api.Entities;
using eShop.Cart.Api.Mapping;
using eShop.Domain.DTOs;

namespace eShop.Cart.Api.Queries.Carts;

internal sealed record GetCartQuery(Guid UserId) : IRequest<Result<CartDto>>;

internal sealed class GetCartQueryHandler(DbClient client) : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly DbClient client = client;

    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<CartEntity>("Carts");
        var cart = await collection.Find(x => x.UserId == request.UserId).FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return new(new NotFoundException($"Cannot find cart with user ID {request.UserId}."));
        }

        return Mapper.ToCartDto(cart);
    }
}