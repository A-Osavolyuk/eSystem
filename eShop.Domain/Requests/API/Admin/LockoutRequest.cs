namespace eShop.Domain.Requests.API.Admin;

public record LockoutRequest
{
    public Guid UserId { get; set; }
    public LockoutReason Reason { get; set; }
    public LockoutPeriod Period { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset? EndDate { get; set; }
}