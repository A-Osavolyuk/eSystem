namespace eShop.Domain.Responses.Api.Auth;

public class UserExistsResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}