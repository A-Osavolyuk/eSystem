using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Entities;

public class ResourceOwnerEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}