namespace eShop.Comments.Api.Entities;

public record CommentEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;

    public string CommentText { get; set; } = string.Empty;
    public List<string> Images { get; set; } = [];
    public int Rating { get; set; }

    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}