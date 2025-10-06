namespace eShop.Domain.Requests.Auth;

public record LockoutRequest
{
    public Guid UserId { get; set; }
    public Guid ReasonId { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Description { get; set; }
    public bool IsPermanent { get; set; }
}