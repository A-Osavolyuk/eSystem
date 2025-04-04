namespace eShop.Domain.Requests.Api.Review;

public record class UpdateReviewRequest
{
    public Guid ReviewId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Rating { get; set; }
}