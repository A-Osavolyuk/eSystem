namespace eShop.Domain.Responses.API.Auth;

public class CheckEmailResponse
{
    public bool IsTaken { get; set; }
    public bool HasLinkedAccount { get; set; }
}