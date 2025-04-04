namespace eShop.Domain.Requests.Api.Comments;

public record UpdateCommentRequest()
{
    public Guid CommentId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string CommentText { get; set; } = string.Empty;
    public List<string> Images { get; set; } =  new List<string>();
    public int Rating { get; set; }
}