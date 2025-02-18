namespace eShop.Cart.Api.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
}