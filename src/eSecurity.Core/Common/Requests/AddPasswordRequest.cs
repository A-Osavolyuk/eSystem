namespace eSecurity.Core.Common.Requests;

public sealed class AddPasswordRequest
{
    public required string Subject { get; set; }
    public required string Password { get; set; }
}