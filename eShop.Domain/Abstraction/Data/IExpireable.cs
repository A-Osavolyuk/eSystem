namespace eShop.Domain.Abstraction.Data;

public interface IExpireable
{
    public DateTimeOffset ExpireDate { get; set; }
}