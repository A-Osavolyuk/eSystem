namespace eSecurity.Core.Common.Requests;

public class ReconfigureAuthenticatorRequest
{
    public required Guid UserId { get; set; }
    public required string Secret { get; set; }
}