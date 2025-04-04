namespace eShop.Domain.Responses.Api.Auth;

public class Change2FaStateResponse
{
    public string Message { get; set; } = string.Empty;
    public bool TwoFactorAuthenticationState { get; set; } = false;
}