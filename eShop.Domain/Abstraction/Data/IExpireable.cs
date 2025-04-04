namespace eShop.Domain.Abstraction.Data;

public interface IExpireable
{
    public DateTime ExpireDate { get; set; }
}