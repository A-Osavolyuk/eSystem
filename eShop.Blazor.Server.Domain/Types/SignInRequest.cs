namespace eShop.Blazor.Server.Domain.Types;

public class SignInRequest
{
    public required Guid UserId { get; set; }
}