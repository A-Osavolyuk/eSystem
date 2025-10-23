namespace eAccount.Blazor.Server.Domain.Requests;

public class SignOutRequest
{
    public required Guid UserId { get; set; }
    public required string AccessToken { get; set; }
}