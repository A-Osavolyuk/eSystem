namespace eSecurity.Core.Common.Requests;

public class SignUpRequest
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}