using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class SellerEntity : IEntity<Guid>
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid UserId { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Article { get; set; } = string.Empty;
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}