namespace eShop.AppHost.Options;

public class MongoDbOptions
{
    public string Name { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
}