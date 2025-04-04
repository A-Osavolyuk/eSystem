namespace eShop.Domain.Requests.Api.Comments;

public record class DeleteCommentsRequest
{
    public Guid ProductId { get; set; }
}