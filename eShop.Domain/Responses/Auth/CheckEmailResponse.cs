namespace eShop.Domain.Responses.Auth;

public class CheckEmailResponse
{
    public bool IsTaken { get; set; }
    public bool IsOwn { get; set; }
    public bool HasLinkedAccount { get; set; }
    public EmailType? Type { get; set; }
}