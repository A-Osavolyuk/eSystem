namespace eShop.Domain.Requests.API.Comments;

public record class DeleteCommentsRequest
{
    public Guid ProductId { get; set; }
}