using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Entities;

public class ScopeEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string Name { get; set; }
    public required string Description { get; set; }
}