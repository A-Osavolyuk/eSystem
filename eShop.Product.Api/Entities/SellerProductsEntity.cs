using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Entities;

public class SellerProductsEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}