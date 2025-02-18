using eShop.Cart.Api.Entities;

namespace eShop.Cart.Api.Commands.Favorites;

internal sealed record UpdateFavoritesCommand(UpdateFavoritesRequest Request)
    : IRequest<Result<UpdateFavoritesResponse>>;

internal sealed class UpdateFavoritesCommandHandler(DbClient client)
    : IRequestHandler<UpdateFavoritesCommand, Result<UpdateFavoritesResponse>>
{
    private readonly DbClient client = client;

    public async Task<Result<UpdateFavoritesResponse>> Handle(UpdateFavoritesCommand request,
        CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<FavoritesEntity>("Favorites");
        var favorites = await collection.Find(x => x.FavoritesId == request.Request.FavoritesId)
            .FirstOrDefaultAsync(cancellationToken);

        if (favorites is null)
        {
            return new(new NotFoundException($"Cannot find favorites with ID {request.Request.FavoritesId}."));
        }

        var newFavorites = new FavoritesEntity()
        {
            CreatedAt = favorites.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            FavoritesId = favorites.FavoritesId,
            ItemsCount = request.Request.ItemsCount,
            UserId = favorites.UserId,
            Items = request.Request.Items
        };

        await collection.ReplaceOneAsync(x => x.FavoritesId == request.Request.FavoritesId, newFavorites,
            cancellationToken: cancellationToken);

        return new UpdateFavoritesResponse()
        {
            Message = "Favorites successfully updated"
        };
    }
}