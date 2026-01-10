namespace eSystem.Core.Data.Entities;

public interface IExpirable
{
    public DateTimeOffset ExpireDate { get; set; }
}