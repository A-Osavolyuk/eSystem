namespace eSecurity.Common.Requests;

public class SignOutRequest
{
    public required Guid UserId { get; set; }
    public required string AccessToken { get; set; }
}