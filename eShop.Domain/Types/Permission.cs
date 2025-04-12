using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.Types;

public class Permission : IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
}