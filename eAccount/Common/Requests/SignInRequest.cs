namespace eAccount.Common.Requests;

public class SignInRequest
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}