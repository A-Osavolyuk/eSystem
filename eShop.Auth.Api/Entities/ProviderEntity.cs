namespace eShop.Auth.Api.Entities;

public class ProviderEntity : IEntity
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}