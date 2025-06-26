namespace eShop.Domain.Responses.API.Comments;

public class GetCommentsResponse
{
    public List<CommentDto> Comments { get; set; } = [];
    public string Message { get; set; } = string.Empty;
}