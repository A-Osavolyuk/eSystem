namespace eShop.Auth.Api.Entities;

public class ResourceEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
}