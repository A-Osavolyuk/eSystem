namespace eShop.Domain.Responses.Api.Admin;

public class UnlockUserResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}