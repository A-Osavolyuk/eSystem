namespace eShop.Domain.Requests;

public class RollbackRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public RollbackField Field { get; set; }
}