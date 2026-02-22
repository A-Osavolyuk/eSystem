namespace eSecurity.Core.Common.Requests;

public sealed class EnableTwoFactorRequest
{
    public required string Subject { get; set; }
}