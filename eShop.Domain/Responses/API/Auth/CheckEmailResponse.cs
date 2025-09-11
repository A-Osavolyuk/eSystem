namespace eShop.Domain.Responses.API.Auth;

public class CheckEmailResponse
{
    public bool IsTaken { get; set; }
    public bool IsOwn { get; set; }
    public bool HasLinkedAccount { get; set; }
    public bool HasTwoFactor { get; set; }
    public EmailType? Type { get; set; }
}