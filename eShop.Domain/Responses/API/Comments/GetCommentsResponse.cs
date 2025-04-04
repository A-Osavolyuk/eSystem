namespace eShop.Domain.Responses.Api.Comments;

public class GetCommentsResponse : ResponseBase
{
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
}