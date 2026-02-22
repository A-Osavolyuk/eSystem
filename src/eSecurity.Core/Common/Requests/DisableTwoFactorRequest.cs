namespace eSecurity.Core.Common.Requests;

public sealed class DisableTwoFactorRequest
{
    public required string Subject { get; set; }
}