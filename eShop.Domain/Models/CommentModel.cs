namespace eShop.Domain.Models;

public class CommentModel : IIdentifiable<Guid>, IAuditable
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<string> Images { get; set; } =  new List<string>();
    public int Rating { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}