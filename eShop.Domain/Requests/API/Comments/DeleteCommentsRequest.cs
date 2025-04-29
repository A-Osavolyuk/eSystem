namespace eShop.Domain.Requests.API.Comments;

public record DeleteCommentsRequest
{
    public Guid ProductId { get; set; }
}