namespace eSecurity.Core.Common.Requests;

public sealed class VerifyCodeRequest
{
    public required string Subject { get; set; }
    public required string Code { get; set; }
}