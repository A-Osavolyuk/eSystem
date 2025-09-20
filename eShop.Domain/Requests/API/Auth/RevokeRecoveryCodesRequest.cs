namespace eShop.Domain.Requests.API.Auth;

public class RevokeRecoveryCodesRequest
{
    public Guid UserId { get; set; }
}