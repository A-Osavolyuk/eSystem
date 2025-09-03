namespace eShop.Domain.Abstraction.Data.Entities;

public interface IExpirable
{
    public DateTimeOffset ExpireDate { get; set; }
}