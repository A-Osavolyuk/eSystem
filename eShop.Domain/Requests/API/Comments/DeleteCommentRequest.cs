namespace eShop.Domain.Requests.Api.Comments;

public record DeleteCommentRequest()
{
    public Guid CommentId { get; set; }
}