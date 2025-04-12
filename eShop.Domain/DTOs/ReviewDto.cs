using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.DTOs;

public class ReviewDto : IAuditable, IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<CommentDto> Comments { get; set; } = [];
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}