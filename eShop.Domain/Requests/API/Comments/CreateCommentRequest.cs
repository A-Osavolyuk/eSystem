namespace eShop.Domain.Requests.API.Comments;

public record CreateCommentRequest()
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string CommentText { get; set; } = string.Empty;
    public List<string> Images { get; set; } = [];
    public int Rating { get; set; }
};