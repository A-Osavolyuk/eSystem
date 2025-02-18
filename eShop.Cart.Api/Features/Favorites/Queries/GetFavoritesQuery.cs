using eShop.Cart.Api.Entities;
using eShop.Cart.Api.Mapping;
using eShop.Domain.DTOs;

namespace eShop.Cart.Api.Queries.Favorites;

internal sealed record GetFavoritesQuery(Guid UserId) : IRequest<Result<FavoritesDto>>;

internal sealed class GetFavoritesQueryHandler(DbClient client)
    : IRequestHandler<GetFavoritesQuery, Result<FavoritesDto>>
{
    private readonly DbClient client = client;

    public async Task<Result<FavoritesDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<FavoritesEntity>("Favorites");
        var favorites = await collection.Find(x => x.UserId == request.UserId).FirstOrDefaultAsync(cancellationToken);

        if (favorites is null)
        {
            return new(new NotFoundException($"Cannot find favorites with user ID {request.UserId}"));
        }

        return Mapper.ToFavoritesDto(favorites);
    }
}