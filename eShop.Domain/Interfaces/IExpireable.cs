namespace eShop.Domain.Interfaces;

public interface IExpirable
{
    public DateTime ExpireDate { get; set; }
}