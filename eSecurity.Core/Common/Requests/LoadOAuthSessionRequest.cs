namespace eSecurity.Core.Common.Requests;

public class LoadOAuthSessionRequest
{
    public required Guid SessionId { get; set; }
    public required string Token { get; set; }
}