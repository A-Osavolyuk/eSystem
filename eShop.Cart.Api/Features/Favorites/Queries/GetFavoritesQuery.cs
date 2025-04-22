using eShop.Cart.Api.Entities;
using eShop.Cart.Api.Mapping;
using eShop.Domain.Common.API;

namespace eShop.Cart.Api.Features.Favorites.Queries;

internal sealed record GetFavoritesQuery(Guid UserId) : IRequest<Result>;

internal sealed class GetFavoritesQueryHandler(DbClient client)
    : IRequestHandler<GetFavoritesQuery, Result>
{
    private readonly DbClient client = client;

    public async Task<Result> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<FavoritesEntity>("Favorites");
        var favorites = await collection.Find(x => x.UserId == request.UserId).FirstOrDefaultAsync(cancellationToken);

        if (favorites is null)
        {
            return Results.NotFound($"Cannot find favorites with user ID {request.UserId}");
        }

        return Result.Success(Mapper.Map(favorites));
    }
}