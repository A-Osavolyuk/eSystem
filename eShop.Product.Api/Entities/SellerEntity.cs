using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class SellerEntity : IEntity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid UserId { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}