namespace eShop.Domain.Responses.Auth;

public class ForgotPasswordResponse
{
    public Guid UserId { get; set; }
    public bool HasPassword { get; set; }
}