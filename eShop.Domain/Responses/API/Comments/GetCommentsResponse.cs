using eShop.Domain.Abstraction.Responses;

namespace eShop.Domain.Responses.API.Comments;

public class GetCommentsResponse : ResponseBase
{
    public List<CommentDto> Comments { get; set; } = [];
}