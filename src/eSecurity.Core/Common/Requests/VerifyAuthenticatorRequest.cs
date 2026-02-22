namespace eSecurity.Core.Common.Requests;

public sealed class VerifyAuthenticatorRequest
{
    public required string Subject { get; set; }
    public required string Code { get; set; }
    public required string Secret { get; set; }
}