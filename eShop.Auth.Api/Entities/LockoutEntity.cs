namespace eShop.Auth.Api.Entities;

public class LockoutEntity : IEntity<Guid>
{
    public Guid Id { get; init; }

    public Guid UserId { get; set; }

    public LockoutReason Reason { get; set; }
    public required string Description { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}