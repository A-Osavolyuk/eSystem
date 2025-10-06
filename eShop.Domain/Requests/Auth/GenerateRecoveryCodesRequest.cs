namespace eShop.Domain.Requests.Auth;

public class GenerateRecoveryCodesRequest
{
    public Guid UserId { get; set; }
}