namespace eShop.Domain.Responses.Auth;

public class ConfirmChangeEmailResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}