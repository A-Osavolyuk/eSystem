namespace eShop.Domain.Responses.Auth;

public class CheckPhoneNumberResponse
{
    public bool IsTaken { get; set; }
    public bool IsOwn { get; set; }
}