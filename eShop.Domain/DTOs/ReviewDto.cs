namespace eShop.Domain.DTOs;

public class ReviewDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<CommentDto> Comments { get; set; } = [];
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}