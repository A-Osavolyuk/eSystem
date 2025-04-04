namespace eShop.Domain.Responses.Api.Admin;

public class DeleteUserAccountResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
}