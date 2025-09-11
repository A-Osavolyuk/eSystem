namespace eShop.Domain.Responses.API.Auth;

public class CheckPhoneNumberResponse
{
    public bool IsTaken { get; set; }
    public bool IsOwn { get; set; }
}