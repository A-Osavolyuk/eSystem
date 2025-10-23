namespace eAccount.Blazor.Server.Domain.Requests;

public class SignInRequest
{
    public required Guid UserId { get; set; }
}