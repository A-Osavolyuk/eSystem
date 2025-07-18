namespace eShop.Domain.Requests;

public class RollbackRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public RollbackAction Action { get; set; }
    public RollbackField Field { get; set; }
}