namespace eSecurity.Core.Common.Requests;

public sealed class ReconfigureAuthenticatorRequest
{
    public required string Secret { get; set; }
}