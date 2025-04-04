using eShop.Cart.Api.Entities;
using eShop.Cart.Api.Mapping;
using eShop.Domain.DTOs;
using eShop.Domain.Enums;

namespace eShop.Cart.Api.Features.Cart.Queries;

internal sealed record GetCartQuery(Guid UserId) : IRequest<Result>;

internal sealed class GetCartQueryHandler(DbClient client) : IRequestHandler<GetCartQuery, Result>
{
    private readonly DbClient client = client;

    public async Task<Result> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<CartEntity>("Carts");
        var cart = await collection.Find(x => x.UserId == request.UserId).FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find cart with user ID {request.UserId}."
            });
        }

        return Result.Success(Mapper.Map(cart));
    }
}