namespace eShop.Auth.Api.Entities;

public class LockoutStateEntity : Entity
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public Guid? ReasonId { get; set; }
    public string? Description { get; set; }
    public bool Enabled { get; set; }
    public bool Permanent { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public LockoutReasonEntity? Reason { get; set; }
}