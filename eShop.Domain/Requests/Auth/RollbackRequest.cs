namespace eShop.Domain.Requests.Auth;

public class RollbackRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public ChangeField Field { get; set; }
}