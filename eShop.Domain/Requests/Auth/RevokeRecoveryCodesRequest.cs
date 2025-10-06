namespace eShop.Domain.Requests.Auth;

public class RevokeRecoveryCodesRequest
{
    public Guid UserId { get; set; }
}