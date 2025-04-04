using eShop.Domain.Abstraction.Data;

namespace eShop.Comments.Api.Entities;

public record class CommentEntity : IIdentifiable<Guid>, IAuditable
{
    public Guid Id { get; init; }
    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;

    public string CommentText { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new List<string>();
    public int Rating { get; set; }

    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}