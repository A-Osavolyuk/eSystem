using eSystem.Core.Data.Entities;

namespace eSystem.Product.Api.Entities;

public class SupplierEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
}