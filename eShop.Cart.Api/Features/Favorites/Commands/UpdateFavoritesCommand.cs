using eShop.Cart.Api.Entities;
using eShop.Domain.Enums;

namespace eShop.Cart.Api.Features.Favorites.Commands;

internal sealed record UpdateFavoritesCommand(UpdateFavoritesRequest Request)
    : IRequest<Result>;

internal sealed class UpdateFavoritesCommandHandler(DbClient client)
    : IRequestHandler<UpdateFavoritesCommand, Result>
{
    private readonly DbClient client = client;

    public async Task<Result> Handle(UpdateFavoritesCommand request,
        CancellationToken cancellationToken)
    {
        var collection = client.GetCollection<FavoritesEntity>("Favorites");
        var favorites = await collection.Find(x => x.Id == request.Request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (favorites is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find favorites with ID {request.Request.Id}."
            });
        }

        var newFavorites = new FavoritesEntity()
        {
            CreateDate = favorites.CreateDate,
            UpdateDate = DateTime.UtcNow,
            Id = favorites.Id,
            ItemsCount = request.Request.ItemsCount,
            UserId = favorites.UserId,
            Items = request.Request.Items
        };

        await collection.ReplaceOneAsync(x => x.Id == request.Request.Id, newFavorites,
            cancellationToken: cancellationToken);

        return Result.Success("Favorites successfully updated");
    }
}