namespace eShop.Domain.DTOs;

public class CommentDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<string> Images { get; set; } = [];
    public int Rating { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}