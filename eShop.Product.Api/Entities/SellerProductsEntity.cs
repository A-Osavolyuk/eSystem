namespace eShop.Product.Api.Entities;

public class SellerProductsEntity : IAuditable
{
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
}