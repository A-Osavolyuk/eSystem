namespace eShop.Domain.Responses.API.Auth;

public class AuthorizeResponse
{
    public Guid UserId { get; set; }
    public required string AccessToken { get; set; }
}