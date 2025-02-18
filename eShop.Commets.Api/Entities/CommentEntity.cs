namespace eShop.Commets.Api.Entities;

public record class CommentEntity
{
    public Guid CommentId { get; set; }
    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;

    public string CommentText { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new List<string>();
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}