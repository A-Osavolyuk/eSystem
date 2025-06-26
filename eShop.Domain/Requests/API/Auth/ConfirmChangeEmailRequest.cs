namespace eShop.Domain.Requests.API.Auth;

public record ConfirmEmailChangeRequest
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
    public string CurrentEmailCode { get; set; } = string.Empty;
    public string NewEmailCode { get; set; } = string.Empty;
}