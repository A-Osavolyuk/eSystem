namespace eShop.Domain.Requests.API.Auth;

public record ConfirmForgotPasswordRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}