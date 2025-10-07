namespace eShop.Domain.Requests.Auth;

public class GeneratePasskeyChallengeRequest
{
    public required Guid UserId { get; set; }
}