namespace eShop.Auth.Api.Entities;

public class ProviderEntity : IEntity<Guid>
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}