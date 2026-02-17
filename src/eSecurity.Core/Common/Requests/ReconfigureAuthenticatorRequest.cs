namespace eSecurity.Core.Common.Requests;

public class ReconfigureAuthenticatorRequest
{
    public required string Subject { get; set; }
    public required string Secret { get; set; }
}