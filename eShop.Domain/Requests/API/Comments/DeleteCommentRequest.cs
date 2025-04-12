namespace eShop.Domain.Requests.API.Comments;

public record DeleteCommentRequest()
{
    public Guid CommentId { get; set; }
}