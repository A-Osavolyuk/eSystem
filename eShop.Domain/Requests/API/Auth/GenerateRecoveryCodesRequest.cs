namespace eShop.Domain.Requests.API.Auth;

public class GenerateRecoveryCodesRequest
{
    public Guid UserId { get; set; }
}