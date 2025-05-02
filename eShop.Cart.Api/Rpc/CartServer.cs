using Cart;
using eShop.Cart.Api.Entities;

namespace eShop.Cart.Api.Rpc;

public class CartServer(IMongoDatabase database, ILogger<CartServer> logger) : CartService.CartServiceBase
{
    private readonly IMongoDatabase database = database;
    private readonly ILogger<CartServer> logger = logger;

    public override async Task<InitiateUserResponse> InitiateUser(InitiateUserRequest request,
        ServerCallContext context)
    {
        try
        {
            logger.LogInformation("Executing RPC InitiateUser");

            var cartCollection = database.GetCollection<CartEntity>("Carts");
            var favoritesCollection = database.GetCollection<FavoritesEntity>("Favorites");

            await cartCollection.InsertOneAsync(new CartEntity() { UserId = Guid.Parse(request.UserId) });
            await favoritesCollection.InsertOneAsync(new FavoritesEntity() { UserId = Guid.Parse(request.UserId) });

            logger.LogInformation("Executed RPC InitiateUser");

            return new InitiateUserResponse()
            {
                Message = "User cart and favorites were successfully created",
                IsSucceeded = true
            };
        }
        catch (Exception ex)
        {
            logger.LogError("Executed RPC InitiateUser with exception");
            return new InitiateUserResponse()
            {
                Message = ex.Message,
                IsSucceeded = false
            };
        }
    }
}