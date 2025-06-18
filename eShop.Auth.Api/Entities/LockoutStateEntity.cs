namespace eShop.Auth.Api.Entities;

public class LockoutStateEntity : IEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    
    public LockoutReason Reason { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public bool IsActive => Enabled && EndDate > DateTimeOffset.UtcNow || Permanent;
    
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }

    public UserEntity User { get; set; } = null!;
}